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
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { }

    public Category(string name, int productTypeId, string? description = null)
    {
        SetName(name);
        ProductTypeId = productTypeId;
        Description = description;
    }

    // ======== Методы изменения состояния ========

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        Name = name;
        Slug = Transliterate(Name);
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetImagePath(string? imagePath)
    {
        ImagePath = imagePath;
    }

    public void SetSlug(string? existingSlug = null)
    {
        if (string.IsNullOrWhiteSpace(existingSlug))
        {
            Slug = Transliterate(Name);
        }
        else
        {
            Slug = existingSlug;
        }
    }

    // ======== Работа с продуктами ========

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public void RemoveProduct(Product product)
    {
        _products.Remove(product);
    }

    public static string Transliterate(string text)
    {
        var translit = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
            {'е', "e"}, {'ё', "e"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
            {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
            {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
            {'у', "u"}, {'ф', "f"}, {'х', "h"}, {'ц', "c"}, {'ч', "ch"},
            {'ш', "sh"}, {'щ', "shch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""},
            {'э', "e"}, {'ю', "yu"}, {'я', "ya"}
        };

        var result = new System.Text.StringBuilder();
        foreach (var c in text.ToLowerInvariant())
        {
            if (translit.ContainsKey(c)) result.Append(translit[c]);
            else if (char.IsLetterOrDigit(c)) result.Append(c);
            else if (char.IsWhiteSpace(c)) result.Append('-');
        }
        return result.ToString();
    }
}
