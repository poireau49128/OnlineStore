namespace Store. Web.Areas.Admin.ViewModels;
public sealed class UserCategoryDiscountViewModel
{
    public int Id { get; init; }
    public string ProductTypeName { get; init; } = null!;
    public string CategoryName { get; init; } = null!;
    public decimal DiscountPercent { get; init; }
    public DateTime? Expiration { get; init; }
}
