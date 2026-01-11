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

    public async Task RemoveAsync(int stockId)
    {
        var stock = await _db.ProductStocks. FindAsync(stockId);
        if (stock != null)
        {
            _db.ProductStocks.Remove(stock);
            await _db. SaveChangesAsync();
        }
    }
}
