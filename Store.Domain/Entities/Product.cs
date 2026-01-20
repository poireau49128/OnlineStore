using System.ComponentModel.DataAnnotations;
using Store.Domain.ValueObjects;

namespace Store.Domain.Entities;

public class Product
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Sku { get; set; } = null!;
    
    [Timestamp]
    public byte[]? RowVersion { get; private set; }

    public Money BasePrice { get; private set; } = null!;

    public string? BaseColor {get; private set;}
    public string? BaseSize {get; private set;}

    public int CategoryId { get; private set; } 
    public Category Category { get; private set; } = null!;
    

    private readonly List<ProductVariant> _variants = new();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

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

        Sku = GenerateSku();
    }
    private string GenerateSku()
    {
        return $"{Transliterate(Name)}-{Guid.NewGuid().ToString("N")[..8]}";
    }

    public ProductVariant AddVariant(string color, string? size = null, Money? overridePrice = null)
    {
        var variant = new ProductVariant(Id, color, size, overridePrice);
        _variants.Add(variant);
        return variant;
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

    public void SetSku()
    {
        Sku = $"{Transliterate(Name)}-{Id}";
    }
}
