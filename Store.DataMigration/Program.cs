using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Store.Persistence;
using Store.Domain.Entities;
using Store.Domain.ValueObjects;
using Store.DataMigration;   // тут лежат OldProduct и OldStoreDbContext


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Data migration started ===");

        // ----- 1. Настраиваем контексты -----

        var oldOptionsBuilder = new DbContextOptionsBuilder<OldStoreDbContext>();
        oldOptionsBuilder.UseSqlServer(
            "Server=bekmanwood.by;Database=bekmanwo_Dosermana;User Id=bekmanwo_bekmanwo;Password=Ugh4Au9A;TrustServerCertificate=True;");

        var newOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        newOptionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=StoreDbDev;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=True");

        using var oldDb = new OldStoreDbContext(oldOptionsBuilder.Options);
        using var newDb = new AppDbContext(newOptionsBuilder.Options);

        // Чтобы не нарваться на двойные данные при повторном запуске — по ситуации:
        // newDb.Database.EnsureDeleted();
        // newDb.Database.Migrate();

        // ----- 2. Склады (Warehouses) -----

        Console.WriteLine("== Migrating warehouses ==");

        Warehouse grodnoWarehouse;
        Warehouse moscowWarehouse;

        if (!newDb.Warehouses.Any())
        {
            grodnoWarehouse = new Warehouse("Гродно", null);
            moscowWarehouse = new Warehouse("Москва", null);

            newDb.Warehouses.AddRange(grodnoWarehouse, moscowWarehouse);
            newDb.SaveChanges();
        }
        else
        {
            grodnoWarehouse = newDb.Warehouses.First(w => w.Name == "Гродно");
            moscowWarehouse = newDb.Warehouses.First(w => w.Name == "Москва");
        }

        // ----- 3. Типы товаров и категории (ProductType, Category) -----

        Console.WriteLine("== Migrating product types & categories ==");

        // Старые товары (примерная структура: Category, SubCategory, и т.п.)
        var oldProducts = oldDb.Products
            .AsNoTracking()
            .ToList();

        // словарь типов по имени
        var productTypesByName = new Dictionary<string, ProductType>(StringComparer.OrdinalIgnoreCase);
        // словарь категорий по (тип, категория)
        var categoriesByTypeAndName = new Dictionary<(string typeName, string categoryName), Category>();

        foreach (var p in oldProducts)
        {
            var typeName = p.Category?.Trim() ?? "Без типа";
            var categoryName = p.SubCategory?.Trim() ?? "Без категории";

            if (!productTypesByName.TryGetValue(typeName, out var productType))
            {
                productType = new ProductType(
                    name: typeName,
                    description: null,
                    sortOrder: 0
                );

                newDb.ProductTypes.Add(productType);
                newDb.SaveChanges();

                productTypesByName[typeName] = productType;
            }

            var key = (typeName, categoryName);
            if (!categoriesByTypeAndName.TryGetValue(key, out var category))
            {
                category = new Category(
                    name: categoryName,
                    productTypeId: productType.Id,
                    description: null
                );

                // Если в Category есть метод/сеттер для ImagePath, можно задать здесь
                // category.SetImagePath(...);

                newDb.Categories.Add(category);
                newDb.SaveChanges();

                categoriesByTypeAndName[key] = category;
            }
        }

        // ----- 4. Продукты и варианты (Product, ProductVariant) -----

        Console.WriteLine("== Migrating products & variants ==");

        // Предположим, что в старой БД каждый ряд — вариант (цвет/размер/цена),
        // сгруппируем по имени + категории.
        var productsGrouped = oldProducts
            .GroupBy(p => new { p.Name, p.Category, p.SubCategory });

        foreach (var group in productsGrouped)
        {
            var any = group.First();

            var typeName = any.Category?.Trim() ?? "Без типа";
            var categoryName = any.SubCategory?.Trim() ?? "Без категории";

            var productType = productTypesByName[typeName];
            var category = categoriesByTypeAndName[(typeName, categoryName)];

            // базовая цена: возьмём первую
            var basePriceValue = any.Price;
            var basePrice = new Money(basePriceValue, "RUB");

            var product = new Product(
                name: group.Key.Name,
                description: any.Description,
                basePrice: basePrice,
                baseColor: null,
                baseSize: null,
                categoryId: category.Id
            );

            newDb.Products.Add(product);
            newDb.SaveChanges();

            foreach (var item in group)
            {
                Money? overridePrice = null;
                if (item.Price != basePriceValue)
                {
                    overridePrice = new Money(item.Price, "RUB");
                }

                var variant = new ProductVariant(
                    productId: product.Id,
                    color: item.Color,
                    size: item.Sizes,
                    overridePrice: overridePrice
                );

                newDb.ProductVariants.Add(variant);
                newDb.SaveChanges();

                // ----- 5. Остатки (StockEntry) -----

                if (item.Quantity_Grodno > 0)
                {
                    var stockGrodno = new StockEntry(
                        productId: product.Id,
                        warehouseId: grodnoWarehouse.Id,
                        quantity: item.Quantity_Grodno
                    );
                    newDb.StockEntries.Add(stockGrodno);
                }

                if (item.Quantity_Moscow > 0)
                {
                    var stockMoscow = new StockEntry(
                        productId: product.Id,
                        warehouseId: moscowWarehouse.Id,
                        quantity: item.Quantity_Moscow
                    );
                    newDb.StockEntries.Add(stockMoscow);
                }

                newDb.SaveChanges();

                // ----- 6. Картинки товаров (ProductImage) -----

                if (!string.IsNullOrEmpty(item.FileName))
                {
                    // создаём картинку
                    var image = new ProductImage(item.FileName, sortOrder: 0)
                        .AttachToVariant(variant.Id);   // привязываем к варианту

                    newDb.ProductImages.Add(image);
                    newDb.SaveChanges();
                }

            }
        }
    }
}