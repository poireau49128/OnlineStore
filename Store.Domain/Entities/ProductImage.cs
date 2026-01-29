namespace Store.Domain.Entities;

public class ProductImage
{
    public int Id { get; private set; }

    public string RelativePath { get; private set; } = null!;
    public int SortOrder { get; private set; }
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

    public ProductImage AttachToVariant(int variantId)
    {
        ProductVariantId = variantId;
        return this;
    }
    public void SetSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder));

        SortOrder = sortOrder;
    }

}
