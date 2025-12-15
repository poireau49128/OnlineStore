using Microsoft.AspNetCore.Identity;

namespace Store.Web.Identity;

public class RussianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
        => new()
        {
            Code = nameof(PasswordTooShort),
            Description = $"Пароль должен содержать минимум {length} символов."
        };

    public override IdentityError PasswordRequiresUpper()
        => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Пароль должен содержать хотя бы одну заглавную букву."
        };

    public override IdentityError PasswordRequiresLower()
        => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Пароль должен содержать хотя бы одну строчную букву."
        };

    public override IdentityError PasswordRequiresDigit()
        => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Пароль должен содержать хотя бы одну цифру."
        };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Пароль должен содержать хотя бы один спецсимвол."
        };

    public override IdentityError DuplicateEmail(string email)
        => new()
        {
            Code = nameof(DuplicateEmail),
            Description = "Пользователь с таким email уже зарегистрирован."
        };

    public override IdentityError InvalidEmail(string email)
        => new()
        {
            Code = nameof(InvalidEmail),
            Description = "Некорректный email."
        };

    public override IdentityError InvalidUserName(string userName)
        => new()
        {
            Code = nameof(InvalidUserName),
            Description = "Некорректное имя пользователя."
        };

    public override IdentityError DuplicateUserName(string userName)
        => new()
        {
            Code = nameof(DuplicateUserName),
            Description = "Пользователь с таким именем уже существует."
        };

    public override IdentityError PasswordMismatch()
        => new()
        {
            Code = nameof(PasswordMismatch),
            Description = "Неверный текущий пароль."
        };

    public override IdentityError InvalidToken()
        => new()
        {
            Code = nameof(InvalidToken),
            Description = "Недействительный токен."
        };

    public override IdentityError UserAlreadyHasPassword()
        => new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = "У пользователя уже есть пароль."
        };
}
