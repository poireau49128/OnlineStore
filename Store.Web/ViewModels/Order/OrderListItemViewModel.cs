namespace Store.Web.ViewModels.Order;

public sealed class OrderListItemViewModel
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = null!;
    public decimal Total { get; init; }
}

public sealed class OrderItemViewModel
{
    public string ProductName { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string? Size { get; init; }
    public string WarehouseName {get; init;} = null!;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public decimal Total { get; init; }
}


public sealed class OrderDetailsViewModel
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = null!;
    public string? Comment { get; init; }

    public List<OrderItemViewModel> Items { get; init; } = new();
    public decimal Total { get; init; }
}
