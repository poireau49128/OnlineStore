using Store.Domain.Entities;

namespace Store.Domain.Entities;

public class ProductType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? ImagePath { get; private set; }
    public int SortOrder { get; private set; }

    public string? Slug { get; private set; }

    private readonly List<Category> _categories = new();
    public IReadOnlyCollection<Category> Categories => _categories;

    

    private ProductType() { }

    public ProductType(string name, string? description = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");
            
        Name = name;
        Description = description;
        SortOrder = sortOrder;
    }
}

