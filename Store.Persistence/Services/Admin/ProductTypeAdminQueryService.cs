using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces.Admin;
using Store.Persistence;

public sealed class ProductTypeAdminQueryService
    : IProductTypeAdminQueryService
{
    private readonly AppDbContext _db;

    public ProductTypeAdminQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductTypeWithCategoriesDto>> GetAllWithCategoriesAsync()
    {
        return await _db.ProductTypes
            .AsNoTracking()
            .Include(t => t.Categories)
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Name)
            .Select(t => new ProductTypeWithCategoriesDto
            {
                Id = t.Id,
                Name = t.Name,
                Categories = t.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProductTypeId = c.ProductTypeId
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
