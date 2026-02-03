using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Order>> GetByUserAsync(string userId)
    {
        return await _db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(v => v.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Warehouse)
            .FirstOrDefaultAsync(o => o.Id == id);
    }


    public Task SaveChangesAsync()
        => _db.SaveChangesAsync();


    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(v => v.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Warehouse)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<OrderWithUserEmail>> GetAllWithEmailsAsync()
    {
        var ordersQuery = _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(v => v.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Warehouse);

        return await ordersQuery
            .Join(_db.Users, 
                order => order.UserId, 
                user => user.Id, 
                (order, user) => new OrderWithUserEmail 
                { 
                    Order = order, 
                    Email = user.Email ?? "N/A" 
                })
            .OrderByDescending(x => x.Order.CreatedAt)
            .ToListAsync();
    }


}
