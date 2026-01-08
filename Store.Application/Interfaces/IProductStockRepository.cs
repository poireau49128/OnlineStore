using Store.Domain.Entities;

public interface IProductStockRepository
{
    Task<ProductStock> GetAsync(int variantId, int warehouseId);
}
