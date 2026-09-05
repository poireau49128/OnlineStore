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
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

    private ProductType() { }

    public ProductType(string name, string? description = null, int sortOrder = 0)
    {
        SetName(name);
        Description = description;
        SortOrder = sortOrder;
    }

    // ======== Методы изменения состояния ========

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        Name = name;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetImagePath(string? path)
    {
        ImagePath = path;
    }

    public void SetSortOrder(int order)
    {
        SortOrder = order;
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

    // ======== Управление категориями ========

    public void AddCategory(Category category)
    {
        _categories.Add(category);
    }

    public void RemoveCategory(Category category)
    {
        _categories.Remove(category);
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
