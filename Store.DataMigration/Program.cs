using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Store.Persistence;
using Store.Domain.Entities;
using Store.Domain.ValueObjects;
using Store.DataMigration;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Data migration started ===");

        var oldOptionsBuilder = new DbContextOptionsBuilder<OldStoreDbContext>();
        oldOptionsBuilder.UseSqlServer(
            "Server=bekmanwood.by;Database=bekmanwo_Dosermana;User Id=bekmanwo_bekmanwo;Password=Ugh4Au9A;TrustServerCertificate=True;");

        var newOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        newOptionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=StoreDb;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=True");

        using var oldDb = new OldStoreDbContext(oldOptionsBuilder.Options);
        using var newDb = new AppDbContext(newOptionsBuilder.Options);

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

        Console.WriteLine("== Migrating product types & categories ==");

        var oldProducts = oldDb.Products.AsNoTracking().ToList();

        var productTypesByName = new Dictionary<string, ProductType>(StringComparer.OrdinalIgnoreCase);
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

                var baseSlug = ProductType.Transliterate(typeName);
                var slug = baseSlug;
                int suffix = 1;

                while (newDb.ProductTypes.Any(pt => pt.Slug == slug))
                {
                    slug = $"{baseSlug}-{suffix}";
                    suffix++;
                }

                productType.SetSlug(slug);

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

                // Генерируем уникальный slug для категории в рамках типа
                var baseSlug = Category.Transliterate(categoryName);
                var slug = baseSlug;
                int suffix = 1;

                while (newDb.Categories.Any(c => c.ProductTypeId == productType.Id && c.Slug == slug))
                {
                    slug = $"{baseSlug}-{suffix}";
                    suffix++;
                }

                category.SetSlug(slug);

                newDb.Categories.Add(category);
                newDb.SaveChanges();

                categoriesByTypeAndName[key] = category;
            }
        }

        Console.WriteLine("== Migrating products & variants ==");

        var productsGrouped = oldProducts.GroupBy(p => new { p.Name, p.Category, p.SubCategory });

        foreach (var group in productsGrouped)
        {
            var any = group.First();
            var typeName = any.Category?.Trim() ?? "Без типа";
            var categoryName = any.SubCategory?.Trim() ?? "Без категории";

            var productType = productTypesByName[typeName];
            var category = categoriesByTypeAndName[(typeName, categoryName)];

            // базовая цена
            var basePrice = Money.From(any.Price, "BYN");

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

            product.SetSku();
            newDb.Products.Update(product);
            newDb.SaveChanges();

            foreach (var item in group)
            {
                Money? overridePrice = null;
                if (item.Price != basePrice.Amount)
                    overridePrice = Money.From(item.Price, "BYN");

                var variant = new ProductVariant(
                    productId: product.Id,
                    color: item.Color,
                    size: item.Sizes,
                    overridePrice: overridePrice
                );

                newDb.ProductVariants.Add(variant);
                newDb.SaveChanges();

                if (item.Quantity_Grodno > 0)
                    newDb.ProductStocks.Add(new ProductStock(product.Id, grodnoWarehouse.Id, item.Quantity_Grodno));

                if (item.Quantity_Moscow > 0)
                    newDb.ProductStocks.Add(new ProductStock(product.Id, moscowWarehouse.Id, item.Quantity_Moscow));

                newDb.SaveChanges();

                if (!string.IsNullOrEmpty(item.FileName))
                {
                    var image = new ProductImage(item.FileName, sortOrder: 0)
                        .AttachToVariant(variant.Id);

                    newDb.ProductImages.Add(image);
                    newDb.SaveChanges();
                }
            }
        }

        Console.WriteLine("=== Data migration finished ===");
    }
}
