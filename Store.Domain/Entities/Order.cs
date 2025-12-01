using Store.Domain.ValueObjects;

namespace Store.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Paid,
    Shipped,
    Completed,
    Cancelled
}

public class Order
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public ApplicationUser User { get; private set; } = null!;
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(string userId, string? comment = null)
    {
        UserId = userId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(int productId, int warehouseId, int quantity, Money unitPrice, string? comment)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        _items.Add(new OrderItem(productId, warehouseId, quantity, unitPrice, comment));
    }

    public Money GetTotal()
    {
        var currency = Items.FirstOrDefault()?.UnitPrice.Currency ?? "RUB";
        var total = Money.Zero(currency);
        foreach (var item in _items)
        {
            total += item.UnitPrice * item.Quantity;
        }
        return total;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        // Здесь можно добавить правила переходов
        Status = newStatus;
    }
}

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public string? Comment { get; private set; }

    private OrderItem() { }

    public OrderItem(int productId, int warehouseId, int quantity, Money unitPrice, string? comment)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        Quantity = quantity;
        UnitPrice = unitPrice;
        Comment = comment;
    }
}