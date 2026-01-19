using Store.Domain.Enums;
using Store.Domain.ValueObjects;

public class AdminOrderListDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public Money TotalPrice { get; set; }
}

public class AdminOrderDetailsDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public IReadOnlyList<OrderStatus> AllowedNextStatuses { get; set; } 
        = Array.Empty<OrderStatus>();

    public Money TotalPrice { get; set; }

    public List<AdminOrderItemDto> Items { get; set; } = new();
}

public class AdminOrderItemDto
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public Money Price { get; set; }
}
