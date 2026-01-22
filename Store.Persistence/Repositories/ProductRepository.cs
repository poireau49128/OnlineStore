using Microsoft.EntityFrameworkCore;
using Store. Application.Interfaces.Admin;
using Store.Domain.Entities;

namespace Store.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Product?> GetByIdWithDetailsAsync(int productId)
    {
        return await _db.Products
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            .Include(p => p. Variants)
                .ThenInclude(v => v.Images)
            .Include(p => p. Variants)
                .ThenInclude(v => v. Stocks)
                    .ThenInclude(s => s. Warehouse)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        return await _db. Products
            .FirstOrDefaultAsync(p => p. Id == productId);
    }

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null)
    {
        var query = _db.Products. Where(p => p.Sku == sku);
        
        if (excludeProductId. HasValue)
            query = query.Where(p => p.Id != excludeProductId);

        return await query.AnyAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db. SaveChangesAsync();
    }
}