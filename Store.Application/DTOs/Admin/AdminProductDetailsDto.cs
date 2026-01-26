using Store.Application.DTOs;
using Store.Domain.ValueObjects;

namespace Store.Application. DTOs.Admin;

public sealed class AdminProductDetailsDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string?  Description { get; init; }
    public string Sku { get; init; } = null!;
    public Money BasePrice { get; init; } = null!;
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public string ProductTypeName { get; init; } = null!;
    public bool IsActive { get; init; }

    public List<AdminVariantDetailsDto> Variants { get; init; } = new();
}

public sealed class AdminVariantDetailsDto
{
    public int Id { get; init; }
    public string Color { get; init; } = null!;
    public string?  Size { get; init; }
    public Money? OverridePrice { get; init; }
    public Money ActualPrice { get; init; } = null!;
    public bool IsActive { get; init; }
    public List<AdminVariantImageDto> Images { get; init; } = new();
    public List<AdminVariantStockDto> Stocks { get; init; } = new();
}

public sealed class AdminVariantImageDto
{
    public int Id { get; init; }
    public string RelativePath { get; init; } = null!;
    public int SortOrder { get; init; }
}


public sealed class AdminVariantStockDto
{
    public int StockId { get; init; }
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = null!;
    public int Quantity { get; init; }
}