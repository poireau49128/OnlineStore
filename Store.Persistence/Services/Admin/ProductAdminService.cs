using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.DTOs.Admin;
using Store. Application.Interfaces.Admin;
using Store.Application.Utilities;
using Store.Domain. Entities;
using Store.Domain.ValueObjects;
using Store.Persistence;

namespace Store.Persistence.Services.Admin;

public sealed class ProductAdminService : IProductAdminService
{
    private readonly IProductRepository _productRepo;
    private readonly IProductImageService _imageService;
    private readonly AppDbContext _db;

    public ProductAdminService(
        IProductRepository productRepo,
        IProductImageService imageService,
        AppDbContext db)
    {
        _productRepo = productRepo;
        _imageService = imageService;
        _db = db;
    }

    public async Task<int> CreateProductWithVariantAsync(CreateProductRequest request)
    {
        // Валидация категории
        var category = await _db.Categories.FirstOrDefaultAsync(c => c. Id == request.CategoryId);
        if (category == null)
            throw new InvalidOperationException("Категория не найдена");

        // Создание товара
        var basePrice = Money.From(request.BasePrice, "BYN");
        var product = new Product(
            name: request.Name,
            basePrice: basePrice,
            categoryId: request.CategoryId,
            description: request.Description);

        // Установка SKU:  если указан администратором, используем его, иначе он будет генериться после сохранения
        if (! string.IsNullOrWhiteSpace(request.Sku))
        {
            // Проверяем уникальность
            var skuExists = await _productRepo. SkuExistsAsync(request.Sku);
            if (skuExists)
                throw new InvalidOperationException($"SKU '{request.Sku}' уже используется другим товаром");

            product. Sku = request.Sku;
        }

        await _productRepo.AddAsync(product);

        // Если SKU не был указан, генерируем по ID товара
        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            product.SetSku();
            await _productRepo.UpdateAsync(product);
        }

        // Создание первого варианта
        var variantPrice = request.FirstVariantPrice. HasValue
            ? Money.From(request. FirstVariantPrice.Value, "BYN")
            : null;

        var variant = product.AddVariant(
            color: request.FirstVariantColor,
            size: request.FirstVariantSize,
            overridePrice: variantPrice);

        await _db.SaveChangesAsync();

        // Загрузка изображений варианта, если они есть
        if (request.FirstVariantImages?. Any() == true)
        {
            await _imageService.UploadVariantImagesAsync(
                variant.Id,
                request.FirstVariantImages);
        }

        return product.Id;
    }

    public async Task<AdminProductDetailsDto?> GetForEditAsync(int productId)
    {
        var product = await _productRepo. GetByIdWithDetailsAsync(productId);
        if (product == null)
            return null;

        return new AdminProductDetailsDto
        {
            Id = product. Id,
            Name = product. Name,
            Description = product.Description,
            Sku = product.Sku,
            BasePrice = product. BasePrice,
            CategoryId = product.CategoryId,
            CategoryName = product.Category. Name,
            ProductTypeName = product.Category.ProductType.Name,
            IsActive = true, // TODO: Добавить IsActive в Product (soft delete)
            Variants = product.Variants
                .OrderBy(v => v.Color)
                .ThenBy(v => v.Size)
                .Select(v => new AdminVariantDetailsDto
                {
                    Id = v.Id,
                    Color = v.Color,
                    Size = v.Size,
                    OverridePrice = v. OverridePrice,
                    ActualPrice = v.GetPrice(product.BasePrice),
                    IsActive = true, // TODO: Добавить IsActive в ProductVariant (soft delete)
                    ImagePaths = v.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.RelativePath)
                        .ToList(),
                    Stocks = v.Stocks
                        .OrderBy(s => s. Warehouse.Name)
                        . Select(s => new AdminVariantStockDto
                        {
                            StockId = s.Id,
                            WarehouseId = s. WarehouseId,
                            WarehouseName = s.Warehouse.Name,
                            Quantity = s.Quantity
                        })
                        .ToList()
                }).ToList()
        };
    }

    public async Task UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await _productRepo.GetByIdAsync(request.  ProductId);
        if (product == null)
            throw new InvalidOperationException("Товар не найден");

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.  Id == request.CategoryId);
        if (category == null)
            throw new InvalidOperationException("Категория не найдена");

        // Используем методы вместо прямого присвоения
        product.UpdateName(request.Name);
        product.UpdateDescription(request.Description);
        product.UpdateBasePrice(Money.From(request.BasePrice, "BYN"));

        // Обновление SKU если указан
        if (!string.IsNullOrWhiteSpace(request. Sku) && request.Sku != product.Sku)
        {
            var skuExists = await _productRepo. SkuExistsAsync(request.Sku, request.ProductId);
            if (skuExists)
                throw new InvalidOperationException($"SKU '{request.Sku}' уже используется другим товаром");

            product. Sku = request.Sku;
        }

        if (request.CategoryId != product.CategoryId)
        {
            product.UpdateCategory(request.CategoryId);
        }

        await _productRepo.UpdateAsync(product);
    }

    public async Task<int> CreateVariantAsync(CreateVariantRequest request)
    {
        var product = await _db.Products
            .Include(p => p. Variants)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product == null)
            throw new InvalidOperationException("Товар не найден");

        // Проверяем, нет ли уже такой комбинации цвета и размера
        var variantExists = product.Variants. Any(v =>
            v.Color. Equals(request.Color, StringComparison.OrdinalIgnoreCase) &&
            v.Size == request.Size);

        if (variantExists)
            throw new InvalidOperationException(
                $"Вариант с цветом '{request.Color}' и размером '{request.Size}' уже существует");

        var overridePrice = request.OverridePrice. HasValue
            ? Money.From(request.OverridePrice.Value, "BYN")
            : null;

        var variant = product.AddVariant(
            color: request.Color,
            size: request.Size,
            overridePrice: overridePrice);

        await _db.SaveChangesAsync();

        // Загрузка изображений
        if (request.Images?.Any() == true)
        {
            await _imageService. UploadVariantImagesAsync(variant.Id, request.Images);
        }

        return variant.Id;
    }

    public async Task UpdateVariantAsync(int variantId, CreateVariantRequest request, List<int>? imagesToDelete)
    {
        var variant = await _db.ProductVariants
            .Include(v => v.Product)
                .ThenInclude(p => p.Variants)
            .FirstOrDefaultAsync(v => v.Id == variantId);

        if (variant == null)
            throw new InvalidOperationException("Вариант не найден");

        var variantExists = variant.Product.Variants.Any(v => 
            v.Id != variantId && 
            v.Color.Equals(request.Color, StringComparison.OrdinalIgnoreCase) && 
            v.Size == request.Size);

        if (variantExists)
            throw new InvalidOperationException($"Другой вариант с цветом '{request.Color}' и размером '{request.Size}' уже существует");



        if (imagesToDelete != null && imagesToDelete.Any())
        {
            var imagesToRemove = variant.Images
                .Where(img => imagesToDelete.Contains(img.Id))
                .ToList();

            foreach (var img in imagesToRemove)
            {
                // Используем ваш ProductImageService для физического удаления файлов
                await _imageService.DeleteImageAsync(img.Id);
            }
        }

        variant.UpdateColor(request.Color);
        variant.UpdateSize(request.Size);
        variant.SetOverridePrice(
            request.OverridePrice.HasValue
            ? Money.From(request.OverridePrice.Value, "BYN")
            : null
        );
        if (request.Images?.Any() == true)
        {
            int nextOrder = variant.Images.Any() ? variant.Images.Max(i => i.SortOrder) + 1 : 0;
            await _imageService.UploadVariantImagesAsync(variant.Id, request.Images, nextOrder);
        }
        
        _db.ProductVariants.Update(variant);
        await _db.SaveChangesAsync();
    }

    public async Task DeactivateVariantAsync(int variantId)
    {
        // TODO:  ВАЖНО!  Добавить IsActive поле в ProductVariant перед использованием
        throw new NotImplementedException(
            "Требуется добавить поле IsActive в ProductVariant (миграция БД)");
    }

    public async Task UpdateStockAsync(UpdateStockRequest request)
    {
        var stock = await _db.ProductStocks
            .FirstOrDefaultAsync(s =>
                s.ProductVariantId == request.ProductVariantId &&
                s. WarehouseId == request.WarehouseId);

        if (stock == null)
        {
            // Создаём новый остаток
            stock = new ProductStock(
                request.ProductVariantId,
                request.WarehouseId,
                request.Quantity);

            _db.ProductStocks.Add(stock);
        }
        else
        {
            // Обновляем существующий через метод
            stock.SetQuantity(request.Quantity);
            _db.ProductStocks.Update(stock);
        }

        await _db. SaveChangesAsync();
    }

    public async Task RemoveStockAsync(int stockId)
    {
        var stock = await _db.ProductStocks. FirstOrDefaultAsync(s => s.Id == stockId);
        if (stock == null)
            throw new InvalidOperationException("Остаток не найден");

        _db.ProductStocks. Remove(stock);
        await _db.SaveChangesAsync();
    }

    public async Task DeactivateProductAsync(int productId)
    {
        // TODO: ВАЖНО! Добавить IsActive поле в Product перед использованием
        throw new NotImplementedException(
            "Требуется добавить поле IsActive в Product (миграция БД)");
    }
}