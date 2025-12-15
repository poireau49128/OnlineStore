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
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(CartItem item)
    {
        _db.CartItems.Add(item);
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
}
