using Store.Domain.Entities;

public interface ICartRepository
{
    Task<List<CartItem>> GetAsync(string userId);
    Task AddAsync(CartItem item);
    Task RemoveAsync(int id);
    Task ClearAsync(string userId);
    Task SaveChangesAsync();
    Task<CartItem?> GetByIdAsync(int id);

}
