using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs.Admin;
using Store.Application.Interfaces.Admin;
using Store.Domain.Entities;
using Store.Persistence;

public sealed class CategoryCommandService : ICategoryCommandService
{
    private readonly AppDbContext _db;

    public CategoryCommandService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CreateCategoryResultDto> CreateAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new InvalidOperationException("Название категории обязательно");

        using var tx = await _db.Database.BeginTransactionAsync();

        ProductType productType;

        if (!string.IsNullOrWhiteSpace(dto.NewProductTypeName))
        {
            var typeName = dto.NewProductTypeName.Trim();

            var typeExists = await _db.ProductTypes
                .AnyAsync(t => t.Name == typeName);

            if (typeExists)
                throw new InvalidOperationException("Тип товара уже существует");

            productType = new ProductType(typeName);
            productType.SetSlug();

            _db.ProductTypes.Add(productType);
            await _db.SaveChangesAsync();
        }
        else if (dto.ProductTypeId.HasValue)
        {
            productType = await _db.ProductTypes
                .FirstOrDefaultAsync(t => t.Id == dto.ProductTypeId.Value)
                ?? throw new InvalidOperationException("Тип товара не найден");
        }
        else
        {
            throw new InvalidOperationException("Тип товара не указан");
        }

        var categoryExists = await _db.Categories.AnyAsync(c =>
            c.ProductTypeId == productType.Id &&
            c.Name == dto.Name);

        if (categoryExists)
            throw new InvalidOperationException("Категория уже существует в этом типе");

        var category = new Category(dto.Name, productType.Id);
        category.SetSlug();

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
        
        return new CreateCategoryResultDto
        {
            Id = category.Id,
            Name = category.Name,
            ProductTypeName = productType.Name
        };
    }
}
