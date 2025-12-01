using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; private set; }
    public string? Address { get; private set; }
    public DateTime? RegisteredAt { get; set; } = DateTime.UtcNow;


    private readonly ICollection<Order>? _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private readonly ICollection<CustomerCategoryDiscount>? _categoryDiscounts = new();
    public IReadOnlyCollection<CustomerCategoryDiscount> CategoryDiscounts => _categoryDiscounts.AsReadOnly();

    private ApplicationUser() { }

    public ApplicationUser(string fullName, string address)
    {
        FullName = fullName;
        Address = address;
    }
}
