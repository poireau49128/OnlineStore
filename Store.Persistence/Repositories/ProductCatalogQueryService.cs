using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces;

namespace Store.Persistence.Repositories;

public sealed class ProductCatalogQueryService : IProductCatalogQueryService
{
    private readonly AppDbContext _db;

    public ProductCatalogQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalogAsync(
        int? categoryId,
        string? searchTerm,
        int? skip = null, 
        int? take = null,
        bool includeAdminData = false)
    {
        var query = _db.Products.AsQueryable();
        if (!includeAdminData)
            query = query.Where(p => p.IsActive);
        query = query.AsNoTracking();
            

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Name.Contains(searchTerm));

        query = query
            .OrderBy(p => p.Category.ProductType.Id)
            .ThenBy(p => p.Category.Name)
            .ThenBy(p => p.Id);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return await query.Select(p => new ProductCatalogItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name ?? "Без категории",
                ProductTypeName = p.Category.ProductType.Name ?? "Без типа",
                Price = p.BasePrice,
                isActive = p.IsActive,
                ImagePath = (includeAdminData
                        ? p.Variants
                        : p.Variants.Where(v => v.IsActive))
                    .OrderBy(v => v.Color)
                    .Select(v => v.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.RelativePath)
                        .FirstOrDefault())
                    .FirstOrDefault(path => path != null) ?? "/img/no-image.png",
                
                VariantsCount = includeAdminData
                    ? p.Variants.Count
                    : null,

                TotalStock = includeAdminData
                    ? p.Variants
                        .SelectMany(v => v.Stocks)
                        .Sum(s => s.Quantity)
                    : null
            })
            .ToListAsync();
    }

    public Task<int> GetCatalogCountAsync(
        int? categoryId,
        string? searchTerm)
    {
        var query = _db.Products.AsQueryable().Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Name.Contains(searchTerm));

        return query.CountAsync();
    }

    public async Task<ProductDetailsDto?> GetDetailsAsync(int productId)
    {
        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .Where(p => p.Id == productId)
            .Select(p => new ProductDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryName = p.Category.Name,
                CategoryId = p.Category.Id,
                ProductTypeName = p.Category.ProductType.Name,
                Sku = p.Sku,
                BasePrice = p.BasePrice,
                Variants = p.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.Color)
                    .Select(v => new ProductVariantDto
                    {
                        Id = v.Id,
                        Color = v.Color,
                        Size = v.Size,
                        Price = v.GetPrice(p.BasePrice),
                        Images = v.Images
                            .OrderBy(i => i.SortOrder)
                            .Select(i => i.RelativePath)
                            .ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
