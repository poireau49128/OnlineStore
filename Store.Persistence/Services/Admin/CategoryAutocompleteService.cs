using Microsoft. EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.DTOs.Admin;
using Store.Application.Interfaces.Admin;

namespace Store.Persistence.Services. Admin;

public sealed class CategoryAutocompleteService : ICategoryAutocompleteService
{
    private readonly AppDbContext _db;

    public CategoryAutocompleteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> SearchCategoriesAsync(string searchTerm, int?  productTypeId = null)
    {
        var query = _db.Categories
            . AsNoTracking()
            .Include(c => c.ProductType)
            .AsQueryable();

        if (productTypeId.HasValue)
            query = query.Where(c => c.ProductTypeId == productTypeId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductTypeId = c.ProductTypeId,
                ProductTypeName = c. ProductType. Name
            })
            .ToListAsync();
    }

    public async Task<List<ProductTypeDto>> GetAllProductTypesAsync()
    {
        return await _db.ProductTypes
            .AsNoTracking()
            .OrderBy(pt => pt.SortOrder)
            .ThenBy(pt => pt.Name)
            .Select(pt => new ProductTypeDto
            {
                Id = pt.Id,
                Name = pt.Name
            })
            .ToListAsync();
    }
}