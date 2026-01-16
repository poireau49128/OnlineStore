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
    public Money TotalPrice { get; private set; }

    private Order() { }

    public Order(string userId, string? comment = null)
    {
        UserId = userId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(int productVariantId, int warehouseId, int quantity, Money unitPrice,
                        decimal discountPercent = 0)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

        var item = new OrderItem(productVariantId, warehouseId, quantity, unitPrice, discountPercent);
        
        item.SetOrder(this);

        _items.Add(item);
    }

    public void RecalculateTotal()
    {
        if (!_items.Any())
        {
            TotalPrice = Money.Zero("BYN");
            return;
        }

        var currency = _items[0].UnitPrice.Currency;
        var total = Money.Zero(currency);

        foreach (var item in _items)
            total += item.TotalPrice;

        TotalPrice = total;
    }


    public void ChangeStatus(OrderStatus newStatus)
    {
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
    
    public Money UnitPrice { get; private set; }     // цена с учётом скидок
    public Money TotalPrice => UnitPrice * Quantity;

    public decimal DiscountPercent { get; private set; } = 0;

    private OrderItem() { }

    public OrderItem(int productVariantId, int warehouseId, int quantity,
                     Money unitPrice, decimal discountPercent = 0)
    {
        ProductVariantId = productVariantId;
        WarehouseId = warehouseId;
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        Quantity = quantity;
        UnitPrice = unitPrice ??  throw new ArgumentNullException(nameof(unitPrice));
        DiscountPercent = discountPercent;
    }

    public Money GetTotal()
    {
        var total = UnitPrice.Amount * Quantity;
        var discounted = total * (1 - DiscountPercent / 100m);
        return Money.From(discounted, UnitPrice.Currency);
    }

    internal void SetOrder(Order order)
    {
        Order = order ??  throw new ArgumentNullException(nameof(order));
        OrderId = order.Id;
    }
}