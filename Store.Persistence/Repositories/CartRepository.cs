using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CartItem>> GetAsync(string userId)
        => await _db.CartItems
            .Where(x => x.UserId == userId)
                .Include(x => x.Warehouse)
                .Include(x => x.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Include(x => x.ProductVariant)
                    .ThenInclude(v => v.Stocks)
                        .ThenInclude(s => s.Warehouse)
            .ToListAsync();

    public async Task AddAsync(CartItem item)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(c =>
            c.UserId == item.UserId &&
            c.ProductVariantId == item.ProductVariantId &&
            c.WarehouseId == item.WarehouseId);

        if (existing != null)
        {
            existing.Increase(item.Quantity);
        }
        else
        {
            _db.CartItems.Add(item);
        }

        await _db.SaveChangesAsync();
    }


    public async Task ClearAsync(string userId)
    {
        _db.CartItems.RemoveRange(
            _db.CartItems.Where(c => c.UserId == userId)
        );
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var item = await _db.CartItems.FindAsync(id);
        if (item != null)
        {
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }    

    public async Task<CartItem?> GetByIdAsync(int id)
    {
        return await _db.CartItems
            .Include(x => x.Warehouse)
            .Include(x => x.ProductVariant)
                .ThenInclude(v => v.Stocks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }


}
