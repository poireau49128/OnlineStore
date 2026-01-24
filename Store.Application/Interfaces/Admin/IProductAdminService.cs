using Store.Application.DTOs. Admin;

namespace Store.Application.Interfaces. Admin;
public interface IProductAdminService
{
    Task<int> CreateProductWithVariantAsync(CreateProductRequest request);
    Task<AdminProductDetailsDto? > GetForEditAsync(int productId);
    Task UpdateProductAsync(UpdateProductRequest request);
    Task<int> CreateVariantAsync(CreateVariantRequest request);
    Task UpdateVariantAsync(int vatiantid, CreateVariantRequest request, List<int>? imagesToDelete);
    Task DeactivateVariantAsync(int variantId);
    Task UpdateStockAsync(UpdateStockRequest request);
    Task RemoveStockAsync(int stockId);
    Task DeactivateProductAsync(int productId);
}