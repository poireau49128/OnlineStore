using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IProductCatalogQueryService
{
    Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalogAsync(
        int? categoryId,
        string? searchTerm,
        int? skip = null,
        int? take = null,
        bool includeAdminData = false);

    Task<int> GetCatalogCountAsync(
        int? categoryId,
        string? searchTerm);

    Task<ProductDetailsDto?> GetDetailsAsync(int productId);
}
