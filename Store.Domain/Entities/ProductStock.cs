namespace Store.Domain.Entities;

public class ProductStock
{
    public int Id { get; private set; }
    public int ProductVariantId { get; private set; }
    public ProductVariant ProductVariant { get; private set; } = null!;
    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int Quantity { get; private set; }

    private ProductStock() { }

    public ProductStock(int productVariantId, int warehouseId, int quantity)
    {
        ProductVariantId = productVariantId;
        WarehouseId = warehouseId;
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");

        Quantity = quantity;
    }

    public bool CanFulfill(int requested) => Quantity >= requested;

    public void Decrease(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive");
        if (qty > Quantity) throw new InvalidOperationException("Not enough stock");
        Quantity -= qty;
    }

    public void Increase(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive");
        Quantity += qty;
    }
}
