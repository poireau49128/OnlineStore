using Store.Domain.ValueObjects;

namespace Store.Application.DTOs;

public sealed class ProductCatalogItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string CategoryName { get; init; } = null!;
    public string ProductTypeName { get; init; } = null!;
    public Money Price { get; init; } = null!;
    public string ImagePath { get; init; } = "/img/no-image.png";

    public int? VariantsCount { get; init; }
    public int? TotalStock { get; init; }
}
