using Store.Application.DTOs;
using X.PagedList;

namespace Store.Web.ViewModels.Product;
public sealed class ProductCatalogViewModel
{
    public IPagedList<ProductCatalogItemDto> Products { get; init; } = null!;
    public List<CategoryDto> Categories { get; init; } = new();
    public int? SelectedCategoryId { get; init; }
    public string? SearchTerm { get; init; }
}
