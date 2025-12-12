using Microsoft.AspNetCore.Identity;
using Store.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; private set; }
    public string? Address { get; private set; }
    public DateTime? RegisteredAt { get; set; } = DateTime.UtcNow;


    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders;

    private readonly List<CustomerCategoryDiscount> _categoryDiscounts = new();
    public IReadOnlyCollection<CustomerCategoryDiscount> CategoryDiscounts => _categoryDiscounts;

    private ApplicationUser() { }

    public ApplicationUser(string fullName, string address)
    {
        FullName = fullName;
        Address = address;
    }
}
