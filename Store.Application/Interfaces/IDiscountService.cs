public interface IDiscountService
{
    Task<decimal> GetCategoryDiscountAsync(
        string userId,
        int categoryId,
        DateTime now);
}
