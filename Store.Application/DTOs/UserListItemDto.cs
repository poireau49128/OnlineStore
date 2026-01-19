namespace Store.Application.DTOs;

public sealed class UserListItemDto
{
    public string Id { get; init; } = default!;
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime? RegisteredAt { get; init; }
}

public sealed class UserWithRolesDto
{
    public string Id { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? FullName { get; init; }

    public IList<string> Roles { get; init; } = [];
}