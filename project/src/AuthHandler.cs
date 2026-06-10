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

    protected AuthenticateResult Fail()
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
        return AuthenticateResult.Fail("Unauthorized");
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if my Auth Cookie is present -> user logged in

        string? authheader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authheader)) return Fail();

        string encodedCredentials = authheader.Substring("Basic ".Length).Trim();
        byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
        string[] credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        string username = credentials[0];
        string password = credentials[1];
        string[] udb = File.ReadAllLines("users.txt")[0].Split(';', 2);
        if (password != udb[1] || username != udb[0]) return Fail();

        // Set the response cookie to log the user in

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