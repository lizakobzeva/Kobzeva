namespace gymBackendC__.WebApi.Auth;

public static class RolePermissions
{
    public static readonly Dictionary<string, List<string>> Map =
        new()
        {
            [Roles.Admin] = new()
            {
                Permissions.Read,
                Permissions.Create,
                Permissions.Update,
                Permissions.Delete
            },
            [Roles.Manager] = new()
            {
                Permissions.Read,
                Permissions.Create,
                Permissions.Update
            },
            [Roles.User] = new()
            {
                Permissions.Read,
                Permissions.Create,
            }
        };
}

