using Store.Domain.Entities;

public class OrderWithUserInfo
{
    public Order Order { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
