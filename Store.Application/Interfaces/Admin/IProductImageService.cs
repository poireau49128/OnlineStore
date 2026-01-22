using Store.Application.DTOs;

namespace Store.Application.Interfaces. Admin;

public interface IProductImageService
{
    Task<List<string>> UploadVariantImagesAsync(
        int productVariantId,
        List<ProductImageFile> files,
        int startingSortOrder = 0);
    Task DeleteImageAsync(int imageId);
    Task ReorderImagesAsync(List<(int ImageId, int NewSortOrder)> orders);
}