namespace Store.Domain.Entities;

public class StockEntry
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int Quantity { get; private set; }

    private StockEntry() { }

    public StockEntry(int productId, int warehouseId, int quantity)
    {
        ProductId = productId;
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
