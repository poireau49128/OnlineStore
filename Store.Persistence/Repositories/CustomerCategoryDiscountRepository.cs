using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;

public sealed class CustomerCategoryDiscountRepository
    : ICustomerCategoryDiscountRepository
{
    private readonly AppDbContext _db;

    public CustomerCategoryDiscountRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<CustomerCategoryDiscount?> GetBestAsync(
        string userId,
        int categoryId,
        DateTime now)
    {
        return _db.CustomerCategoryDiscount
            .Where(d => d.UserId == userId)
            .Where(d => d.CategoryId == categoryId)
            .Where(d => d.Expiration == null || d.Expiration > now)
            .OrderByDescending(d => d.DiscountPercent)
            .FirstOrDefaultAsync();
    }
}
