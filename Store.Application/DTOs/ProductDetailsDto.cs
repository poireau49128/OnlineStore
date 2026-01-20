using Store.Domain.ValueObjects;
using Store.Application.DTOs;

namespace Store.Application.DTOs;

public sealed class ProductDetailsDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CategoryName { get; init; } = null!;
    public string ProductTypeName { get; init; } = null!;
    public string? Sku { get; init; }
    public Money BasePrice { get; init; } = null!;
    public IReadOnlyList<ProductVariantDto> Variants { get; init; } = [];
    public int CategoryId { get; init; }
}
