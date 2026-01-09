namespace gymBackendC__.WebApi.Auth;

public interface ICurrentUser
{
    int UserId { get; }
    List<string> UserPermissions { get; }
}
