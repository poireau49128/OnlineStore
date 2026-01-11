using Store.Domain.Entities;
using Store.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Order order)
    {      
        _db.Orders.Add(order);        
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _db.SaveChangesAsync();
}
