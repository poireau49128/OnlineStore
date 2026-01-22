namespace Store.Web.Areas.Admin.ViewModels;

public sealed class ProductListItemViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public decimal Price { get; init; }
    public int VariantsCount { get; init; }
    public int TotalStock { get; init; }
    public bool IsActive { get; init; }
}