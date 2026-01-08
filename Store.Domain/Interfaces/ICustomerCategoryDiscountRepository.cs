using Store.Domain.Entities;

public interface ICustomerCategoryDiscountRepository
{
    Task<CustomerCategoryDiscount?> GetBestAsync(
        string userId,
        int categoryId,
        DateTime now);
}
