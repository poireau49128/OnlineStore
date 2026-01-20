using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces;

namespace Store.Persistence.Repositories;

public sealed class ProductStockQueryService
    : IProductStockQueryService
{
    private readonly AppDbContext _db;

    public ProductStockQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductStockDto>> GetByVariantAsync(int productVariantId)
    {
        return await _db.ProductStocks
            .AsNoTracking()
            .Where(s => s.ProductVariantId == productVariantId)
            .Include(s => s.Warehouse)
            .Select(s => new ProductStockDto
            {
                Warehouse = s.Warehouse,
                Quantity = s.Quantity
            })
            .ToListAsync();
    }
}
