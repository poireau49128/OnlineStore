using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IProductCatalogQueryService
{
    Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalogAsync(
        int? categoryId,
        string? searchTerm,
        int skip,
        int take);

    Task<int> GetCatalogCountAsync(
        int? categoryId,
        string? searchTerm);

    Task<ProductDetailsDto?> GetDetailsAsync(int productId);
}
