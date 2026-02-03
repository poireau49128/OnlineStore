public interface ICustomerCategoryDiscountAdminService
{
    Task<IReadOnlyList<CustomerCategoryDiscount>> GetUserDiscountsAsync(string userId);
    Task AddAsync(string userId, int categoryId, decimal percent, DateTime? expiration);
    Task RemoveAsync(int discountId);
}
