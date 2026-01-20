using Store.Domain.Entities;
using Store.Domain.ValueObjects;

namespace Store.Application.DTOs;

public sealed class ProductVariantDto
{
    public int Id { get; init; }
    public string Color { get; init; } = null!;
    public string? Size { get; init; }
    public Money Price { get; init; } = null!;
    public IReadOnlyList<string> Images { get; init; } = [];
}
