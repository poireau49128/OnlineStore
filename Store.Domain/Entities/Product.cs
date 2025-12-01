using Store.Domain.ValueObjects;

namespace Store.Domain.Entities;

public class Product
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Sku { get; private set; } = null!;
    public byte[]? RowVersion { get; private set; }

    public Money BasePrice { get; private set; }

    public string? BaseColor {get; private set;}
    public string? BaseSize {get; private set;}

    public int CategoryId { get; private set; } 
    public Category Category { get; private set; } = null!;
    

    private readonly List<ProductVariant> _variants = new();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private readonly List<ProductStock> _stocks = new();
    public IReadOnlyCollection<ProductStock> Stocks => _stocks.AsReadOnly();

    private Product() { }

    public Product(
        string name,
        Money basePrice,
        int categoryId,
        string? description = null,
        string? baseColor = null,
        string? baseSize = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Name = name;
        BasePrice = basePrice;
        CategoryId = categoryId;
        Description = description;
        BaseColor = baseColor;
        BaseSize = baseSize;
    }

    public ProductVariant AddVariant(string color, string? size = null, Money? overridePrice = null)
    {
        var variant = new ProductVariant(Id, color, size, overridePrice);
        _variants.Add(variant);
        return variant;
    }

    public ProductImage AddImage(string relativePath, int sortOrder = 0)
    {
        var image = new ProductImage(relativePath, sortOrder)
            .AttachToProduct(Id);
        _images.Add(image);
        return image;
    }
}
