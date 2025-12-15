using System.ComponentModel.DataAnnotations;

namespace Store.Web.ViewModels.Account;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? FullName { get; set; } = null!;

    public string? Address { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; } = null!;
}
