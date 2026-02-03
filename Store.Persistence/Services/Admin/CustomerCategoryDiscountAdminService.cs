using Microsoft.EntityFrameworkCore;
using Store.Persistence;

public sealed class CustomerCategoryDiscountAdminService
    : ICustomerCategoryDiscountAdminService
{
    private readonly AppDbContext _db;

    public CustomerCategoryDiscountAdminService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CustomerCategoryDiscount>> GetUserDiscountsAsync(string userId)
    {
        return await _db.CustomerCategoryDiscount
            .Include(d => d.Category)
                .ThenInclude(c => c.ProductType)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.DiscountPercent)
            .ToListAsync();
    }

    public async Task AddAsync(
        string userId,
        int categoryId,
        decimal percent,
        DateTime? expiration)
    {
        if (percent <= 0 || percent > 100)
            throw new ArgumentException("Некорректный процент скидки");

        var discount = new CustomerCategoryDiscount(
            code: null,
            userId: userId,
            discountPercent: percent,
            categoryId: categoryId,
            expiration: expiration);

        _db.CustomerCategoryDiscount.Add(discount);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int discountId)
    {
        var discount = await _db.CustomerCategoryDiscount.FindAsync(discountId);
        if (discount == null)
            return;

        _db.CustomerCategoryDiscount.Remove(discount);
        await _db.SaveChangesAsync();
    }
}
