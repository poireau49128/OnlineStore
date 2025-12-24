namespace Store.Web.ViewModels.Cart;

public class CartItemViewModel
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? Size { get; set; }

    public int Quantity { get; set; }

    public string WarehouseName { get; set; } = null!;
    public int AvailableQuantity { get; set; }


    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public string Currency { get; set; } = "BYN";
}
