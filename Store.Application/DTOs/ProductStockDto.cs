using Store.Domain.Entities;

namespace Store.Application.DTOs;

public sealed class ProductStockDto
{
    public Warehouse Warehouse { get; init; }
    public int Quantity { get; init; }
}
