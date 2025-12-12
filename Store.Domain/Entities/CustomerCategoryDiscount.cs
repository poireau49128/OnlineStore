using Store.Domain.Entities;

public class CustomerCategoryDiscount
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string? Code { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public DateTime? Expiration { get; private set; }

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private CustomerCategoryDiscount() { }

    public CustomerCategoryDiscount(string? code, string userId, decimal discountPercent, int categoryId, DateTime? expiration = null)
    {
        Code = code;
        UserId = userId;
        DiscountPercent = discountPercent;
        CategoryId = categoryId;
        Expiration = expiration;
    }
}
