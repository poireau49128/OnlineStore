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

    public async Task UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.Id)
            ?? throw new InvalidOperationException("Категория не найдена");

        var exists = await _db.Categories.AnyAsync(c =>
            c.Id != dto.Id &&
            c.ProductTypeId == category.ProductTypeId &&
            c.Name == dto.Name
        );

        if (exists)
            throw new InvalidOperationException("Категория с таким именем уже существует");

        category.SetName(dto.Name);
        category.SetSlug();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId)
            ?? throw new InvalidOperationException("Категория не найдена");

        if (category.Products.Any())
            throw new InvalidOperationException("Нельзя удалить категорию с товарами");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateProductTypeAsync(int id, string name)
    {
        var type = await _db.ProductTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new InvalidOperationException("Тип товара не найден");

        var exists = await _db.ProductTypes.AnyAsync(t =>
            t.Id != id && t.Name == name
        );

        if (exists)
            throw new InvalidOperationException("Тип товара с таким именем уже существует");

        type.SetName(name);
        type.SetSlug();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteProductTypeAsync(int id)
    {
        var type = await _db.ProductTypes
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new InvalidOperationException("Тип товара не найден");

        if (type.Categories.Any())
            throw new InvalidOperationException("Нельзя удалить тип с категориями");

        _db.ProductTypes.Remove(type);
        await _db.SaveChangesAsync();
    }


}
