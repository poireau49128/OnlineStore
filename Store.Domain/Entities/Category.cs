using Store.Domain.Entities;

namespace Store.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? ImagePath { get; private set; }
    public string? Description { get; private set; }

    public string? Slug { get; private set; }

    public int ProductTypeId { get; private set; }
    public ProductType ProductType { get; private set; } = null!;

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products;

    private Category() { }

    public Category(string name, int productTypeId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");
            
        Name = name;
        ProductTypeId = productTypeId;
        Description = description;
    }
}

