using Store.Domain.Entities;

namespace Store. Application.Interfaces.Admin;

public interface IProductRepository
{
    Task<Product? > GetByIdWithDetailsAsync(int productId);
    Task<Product?> GetByIdAsync(int productId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> SkuExistsAsync(string sku, int?  excludeProductId = null);
    Task SaveChangesAsync();
}