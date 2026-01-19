using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IUserQueryService
{
    Task<IReadOnlyList<UserListItemDto>> GetAllAsync();
}

public interface IUserDetailsQueryService
{
    Task<UserWithRolesDto?> GetByIdAsync(string userId);
}