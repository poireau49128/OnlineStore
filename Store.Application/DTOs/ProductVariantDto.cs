using Store.Domain.ValueObjects;

public class ProductVariantDto
{
    public string Color { get; set; } = null!;
    public string? Size { get; set; }
    public Money Price { get; set; } = null!;
    public List<string> ImagePaths { get; set; } = new();
}