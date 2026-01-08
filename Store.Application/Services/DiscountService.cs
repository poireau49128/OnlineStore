public sealed class DiscountService : IDiscountService
{
    private readonly ICustomerCategoryDiscountRepository _repo;

    public DiscountService(ICustomerCategoryDiscountRepository repo)
    {
        _repo = repo;
    }

    public async Task<decimal> GetCategoryDiscountAsync(
        string userId,
        int categoryId,
        DateTime now)
    {
        var discount = await _repo.GetBestAsync(userId, categoryId, now);
        return discount?.DiscountPercent ?? 0m;
    }
}
