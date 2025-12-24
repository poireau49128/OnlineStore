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

    public void AddItem(int productId, int warehouseId, int quantity, Money unitPrice,
                            decimal discountPercent, string? comment)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        _items.Add(new OrderItem(Id, productId, warehouseId, quantity, unitPrice, discountPercent, comment));
    }

    public Money GetTotal()
    {
        var currency = Items.FirstOrDefault()?.UnitPrice.Currency ?? "BYN";
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
    
    public int ProductVariantId { get; private set; }
    public ProductVariant ProductVariant { get; private set; } = null!;

    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;

    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }

    public decimal DiscountPercent { get; private set; } = 0;
    public string? Comment { get; private set; }

    private OrderItem() { }

    public OrderItem(int orderId, int productVariantId, int warehouseId, int quantity,
                        Money unitPrice, decimal discountPercent = 0, string? comment = null)
    {
        OrderId = orderId;
        ProductVariantId = productVariantId;
        WarehouseId = warehouseId;
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercent = discountPercent;
        Comment = comment;
    }

    public Money GetTotal()
    {
        var total = UnitPrice.Amount * Quantity;
        var discounted = total * (1 - DiscountPercent / 100m);
        return Money.From(discounted, UnitPrice.Currency);
    }
}