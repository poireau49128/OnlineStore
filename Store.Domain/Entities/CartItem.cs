namespace Store.Domain.Entities;

public class CartItem
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public int ProductId { get; private set; }
    public Product Product {get; private set;}
    public int WarehouseId { get; private set; }
    public int Quantity { get; private set; }

    private CartItem() { }

    public CartItem(string userId, int productId, int warehouseId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

        UserId = userId;
        ProductId = productId;
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
