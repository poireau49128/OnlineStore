namespace Store.Application.Commands;

public sealed record ChangeUserAdminRoleCommand(
    string UserId,
    bool IsAdmin,
    string PerformedByUserId
);
