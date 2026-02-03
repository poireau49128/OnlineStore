using Store.Domain.Entities;

namespace Store.Web.Areas.Admin.ViewModels;

public sealed class UserDetailsViewModel
{
    public string Id { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? FullName { get; init; }
    public string? Address { get; init; }
    public List<UserOrderDto> Orders { get; set; } = new();
    public bool IsAdmin { get; init; }
    public List<UserCategoryDiscountViewModel> CategoryDiscounts { get; init; } = new();
    public List<ProductTypeWithCategoriesDto> ProductTypes { get; init; } = new();

}
