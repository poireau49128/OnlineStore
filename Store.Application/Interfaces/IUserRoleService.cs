using Store.Application.Commands;

namespace Store.Application.Interfaces;

public interface IUserRoleService
{
    Task SetAdminRoleAsync(ChangeUserAdminRoleCommand command);
    Task ResetPasswordAsync(string userId, string newPassword);
}
