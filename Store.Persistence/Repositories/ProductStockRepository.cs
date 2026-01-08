using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;

public sealed class ProductStockRepository : IProductStockRepository
{
    private readonly AppDbContext _db;

    public ProductStockRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<ProductStock> GetAsync(int variantId, int warehouseId)
    {
        return _db.ProductStocks
            .Include(s => s.ProductVariant)
                .ThenInclude(v => v.Product)
            .FirstAsync(s =>
                s.ProductVariantId == variantId &&
                s.WarehouseId == warehouseId);
    }
}
