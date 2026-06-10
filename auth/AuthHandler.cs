using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILoggerFactory logger;

    [Obsolete]
    public AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        this.logger = logger;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authheader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authheader))
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
            return AuthenticateResult.Fail("Unauthorized");
        }
        string encodedCredentials = authheader.Substring("Basic ".Length).Trim();
        byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
        string[] credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        string username = credentials[0];
        string password = credentials[1];
        if (password != "demo123" || username != "admin")
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
            return AuthenticateResult.Fail("Unauthorized");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}