using Store.Domain.Entities;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task SaveChangesAsync();
    Task<List<Order>> GetByUserAsync(string userId);
    Task<Order?> GetByIdAsync(int id);
}
