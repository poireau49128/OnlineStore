using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IProductStockQueryService
{
    Task<IReadOnlyList<ProductStockDto>> GetByVariantAsync(int productVariantId);
}
