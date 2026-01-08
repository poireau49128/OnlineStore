using Store.Domain.Entities;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}
