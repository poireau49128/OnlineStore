using Store.Domain.ValueObjects;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? BaseColor { get; set; }
    public string? BaseSize { get; set; }

    public Money BasePrice { get; set; } = null!;
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<string> ImagePaths { get; set; } = new();
}