namespace Store.Domain.Entities;

public class CartItem
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public int ProductVariantId { get; private set; }
    public ProductVariant ProductVariant {get; private set;} = null!;
    public int WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public int Quantity { get; private set; }

    private CartItem() { }

    public CartItem(string userId, int productVariantId, int warehouseId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

        UserId = userId;
        ProductVariantId = productVariantId;
        WarehouseId = warehouseId;
        Quantity = quantity;
    }

    public void Increase(int qty)
    {
        if (qty <= 0) throw new ArgumentException();
        Quantity += qty;
    }

    public void SetQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentException();
        Quantity = qty;
    }
}
