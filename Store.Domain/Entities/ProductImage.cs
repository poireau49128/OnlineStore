namespace Store.Domain.Entities;

public class ProductImage
{
    public int Id { get; private set; }

    public string RelativePath { get; private set; } = null!;
    public int SortOrder { get; private set; }

    // Картинка может принадлежать продукту
    public int? ProductId { get; private set; }
    public Product? Product { get; private set; }

    // … или варианту продукта (цвет/размер)
    public int? ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }

    private ProductImage() { }

    public ProductImage(string relativePath, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("RelativePath is required", nameof(relativePath));

        RelativePath = relativePath;
        SortOrder = sortOrder;
    }

    public ProductImage AttachToProduct(int productId)
    {
        ProductId = productId;
        ProductVariantId = null;
        return this;
    }

    public ProductImage AttachToVariant(int variantId)
    {
        ProductVariantId = variantId;
        ProductId = null;
        return this;
    }
}
