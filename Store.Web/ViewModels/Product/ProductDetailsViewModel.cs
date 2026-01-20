using Store.Application.DTOs;
using Store.Domain.ValueObjects;

namespace Store.Web.ViewModels.Product;
public sealed class ProductDetailsViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    public string CategoryName { get; init; } = null!;
    public int CategoryId { get; init; }

    public string ProductTypeName { get; init; } = null!;
    public string Sku { get; init; } = null!;

    public Money BasePrice { get; init; } = null!;

    public IReadOnlyList<ProductVariantDto> Variants { get; init; }
        = Array.Empty<ProductVariantDto>();
}
