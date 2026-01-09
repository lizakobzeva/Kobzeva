using gymBackendC__.WebApi.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace gymBackendC__.WebApi.Auth;

public class ApiKeyAuthScheme : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AppDbContext _dbContext;
    private const string API_KEY_HEADER = "X-API-KEY";

    public ApiKeyAuthScheme(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AppDbContext dbContext
    ) : base(options, logger, encoder, clock)
    {
        _dbContext = dbContext;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeaderValues))
            return Task.FromResult(AuthenticateResult.NoResult());

        var apiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        var key = _dbContext.ApiKeys.FirstOrDefault(k => k.Key == apiKey && k.IsActive && k.Expiration > DateTime.UtcNow);
        if (key == null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired or deactivated API Key"));

        var claims = new List<Claim>
        {
            new Claim("apiKeyId", key.Id.ToString())
        };
        
        var permissions = RolePermissions.Map[Roles.Admin];
        claims.AddRange(permissions.Select(p =>
            new Claim("permissions", p)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}