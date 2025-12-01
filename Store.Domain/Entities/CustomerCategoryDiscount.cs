public class CustomerCategoryDiscount
{
    public int Id { get; private set; }
    public string CustomerId { get; private set; } = null!;
    public ApplicationUser Customer { get; private set; } = null!;
    public string? Code { get; private set; };
    public decimal Percentage { get; private set; }
    public DateTime? Expiration { get; private set; }

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private CustomerCategoryDiscount() { }

    public CustomerCategoryDiscount(string? code, string customerId, decimal percentage, int categoryId, DateTime? expiration = null)
    {
        Code = code;
        CustomerId = customerId;
        Percentage = percentage;
        CategoryId = categoryId;
        Expiration = expiration;
    }
}
