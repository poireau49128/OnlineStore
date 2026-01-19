using Microsoft.AspNetCore.Identity;
using Store.Application.Commands;
using Store.Application.Interfaces;


namespace Store.Persistence.Identity;

public sealed class UserRoleService : IUserRoleService
{
    private const string AdminRole = "Admin";

    private readonly UserManager<ApplicationUser> _userManager;

    public UserRoleService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task SetAdminRoleAsync(ChangeUserAdminRoleCommand command)
    {
        if (command.UserId == command.PerformedByUserId && !command.IsAdmin)
            throw new InvalidOperationException(
                "Нельзя снять роль администратора с самого себя");

        var user = await _userManager.FindByIdAsync(command.UserId)
                   ?? throw new InvalidOperationException("Пользователь не найден");

         if (command.IsAdmin)
        {
            if (!await _userManager.IsInRoleAsync(user, AdminRole))
                await _userManager.AddToRoleAsync(user, AdminRole);
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(user, AdminRole);
        }
    }
}
