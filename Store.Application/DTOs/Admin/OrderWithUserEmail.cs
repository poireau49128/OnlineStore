using Store.Domain.Entities;

public class OrderWithUserEmail
{
    public Order Order { get; set; } = null!;
    public string Email { get; set; } = null!;
}
