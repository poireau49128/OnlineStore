using System.ComponentModel.DataAnnotations;

namespace Store.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = null!;

    public string? FullName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ReturnUrl { get; set; }
}
