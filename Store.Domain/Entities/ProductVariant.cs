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

    public Money GetPrice(Money basePrice) => OverridePrice ?? basePrice;

    public ProductImage AddImage(string relativePath, int sortOrder = 0)
    {
        var image = new ProductImage(relativePath, sortOrder)
            .AttachToVariant(Id);
        _images.Add(image);
        return image;
    }
}
