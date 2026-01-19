using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.Interfaces;

namespace Store.Persistence.Identity;

public sealed class UserQueryService : IUserQueryService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserQueryService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetAllAsync()
    {
        return await _userManager.Users
            .AsNoTracking()
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Address = u.Address,
                PhoneNumber = u.PhoneNumber,
                RegisteredAt = u.RegisteredAt
            })
            .ToListAsync();
    }
}

public sealed class UserDetailsQueryService : IUserDetailsQueryService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserDetailsQueryService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserWithRolesDto?> GetByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles
        };
    }
}
