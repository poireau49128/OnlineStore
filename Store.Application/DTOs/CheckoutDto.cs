public sealed class CheckoutItemDto
{
    public int ProductVariantId { get; init; }
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = null!;
    public int Quantity { get; init; }
}

public sealed class CheckoutCommand
{
    public string UserId { get; init; } = null!;
    public string? Comment { get; init; }
    public IReadOnlyCollection<CheckoutItemDto> Items { get; init; }
        = Array.Empty<CheckoutItemDto>();
}
