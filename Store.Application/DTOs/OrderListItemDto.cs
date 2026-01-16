using Store.Domain.Entities;
using Store.Domain.ValueObjects;

public sealed class OrderListDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public Money Total { get; init; }
}

public sealed class OrderDetailsDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public string Comment { get; init; } = null!;
    public Money Total { get; init; }
}

public sealed class OrderItemDto
{
    public string ProductName { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string? Size { get; init; }
    public string WarehouseName { get; init; } = null!;
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }
    public decimal Discount { get; init; }
    public Money TotalPrice { get; init; }
}
