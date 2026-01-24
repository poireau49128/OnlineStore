using Store.Domain.Entities;
using Store.Domain.ValueObjects;

namespace Store.Domain.Entities;

public class ProductVariant
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public string Color { get; private set; } = null!;
    public string? Size { get; private set; }

    // Своя цена, если отличается от базовой
    public Money? OverridePrice { get; private set; }

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private readonly List<ProductStock> _stocks = new();
    public IReadOnlyCollection<ProductStock> Stocks => _stocks.AsReadOnly();

    private ProductVariant() { }

    public ProductVariant(int productId, string color, string? size = null, Money? overridePrice = null)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color is required", nameof(color));

        ProductId = productId;
        Color = color;
        Size = size;
        OverridePrice = overridePrice;
    }

    public ProductStock AddStock(int warehouseId, int quantity)
    {
        var stock = new ProductStock(Id, warehouseId, quantity);
        _stocks.Add(stock);
        return stock;
    }

    public Money GetPrice(Money basePrice) => OverridePrice ?? basePrice;

    public void SetOverridePrice(Money? price)
    {
        OverridePrice = price;
    }


    public ProductImage AddImage(string relativePath, int sortOrder = 0)
    {
        var image = new ProductImage(relativePath, sortOrder)
            .AttachToVariant(Id);

        _images.Add(image);
        return image;
    }

     public void UpdateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Цвет обязателен", nameof(color));
        Color = color;
    }

    public void UpdateSize(string?  size)
    {
        Size = size;
    }

    // public void UpdateImage(string relativePath, int sortOrder = 0)
    // {
    //     if (categoryId <= 0)
    //         throw new ArgumentException("Invalid category", nameof(categoryId));
    //     CategoryId = categoryId;
    // }

}
