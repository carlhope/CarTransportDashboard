namespace CarTransportDashboard.Models.Users;

public enum UserRoles
{
    Admin,
    Driver,
    Dispatcher
}
public static class RoleConstants
{
    public const string Admin = nameof(UserRoles.Admin);
    public const string Driver = nameof(UserRoles.Driver);
    public const string Dispatcher = nameof(UserRoles.Dispatcher);
}
