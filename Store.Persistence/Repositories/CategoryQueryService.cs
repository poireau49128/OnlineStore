using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces;
using Store.Persistence;

namespace Store.Persistence.Repositories
{
    public sealed class CategoryQueryService : ICategoryQueryService
    {
        private readonly AppDbContext _db;

        public CategoryQueryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _db.Categories
                .Include(c => c.ProductType)
                .OrderBy(с => с.ProductType.Id)
                .ThenBy(с => с.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c.ProductType.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int categoryId)
        {
            return await _db.Categories
                .Include(c => c.ProductType)
                .Where(c => c.Id == categoryId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c.ProductType.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}
