using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces;

namespace Store.Persistence.Repositories;

public sealed class ProductCatalogQueryService
    : IProductCatalogQueryService
{
    private readonly AppDbContext _db;

    public ProductCatalogQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalogAsync(
        int? categoryId,
        string? searchTerm,
        int skip,
        int take)
    {
        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Images)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Name.Contains(searchTerm));

        return await query
            .OrderBy(p => p.Category.ProductType.Id)
            .ThenBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .Select(p => new ProductCatalogItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                ProductTypeName = p.Category.ProductType.Name,
                Price = p.BasePrice,
                ImagePath = p.GetMainImagePath()
            })
            .ToListAsync();
    }

    public Task<int> GetCatalogCountAsync(
        int? categoryId,
        string? searchTerm)
    {
        var query = _db.Products.AsQueryable();

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
                ProductTypeName = p.Category.ProductType.Name,
                Sku = p.Sku,
                BasePrice = p.BasePrice,
                Variants = p.Variants.OrderBy(v => v.Color)
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
