namespace Identity.Application.Common.Audit;

public static class AuditActions
{
    public const string UserRegistered = "user.registered";
    public const string UserLoginSuccess = "user.login.success";
    public const string UserLoginFailed = "user.login.failed";
    public const string UserLogout = "user.logout";
    public const string UserRefreshTokenRotated = "user.refresh.rotated";
    public const string UserRefreshTokenInvalid = "user.refresh.invalid";

    public const string UserPasswordChanged = "user.password.changed";
    public const string UserPasswordResetRequested = "user.password.reset.requested";
    public const string UserPasswordResetCompleted = "user.password.reset.completed";
    public const string UserLoginChanged = "user.login.changed";

    public const string UserDeactivated = "user.deactivated";
    public const string UserActivated = "user.activated";
    public const string UserBlocked = "user.blocked";
    public const string UserUnblocked = "user.unblocked";

    public const string SessionRevoked = "session.revoked";
    public const string SessionsRevokedAll = "session.revoked.all";

    public const string RoleCreated = "role.created";
    public const string RoleDeleted = "role.deleted";
    public const string RoleRenamed = "role.renamed";
    public const string RoleAssigned = "role.assigned";
    public const string RoleRevoked = "role.revoked";

    public const string CommandExecuted = "command.executed";
}
