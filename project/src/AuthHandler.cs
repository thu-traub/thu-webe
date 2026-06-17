using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger logger;

    [Obsolete]
    public AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        this.logger = logger.CreateLogger<AuthHandler>();
    }

    protected AuthenticateResult Fail()
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
        return AuthenticateResult.Fail("Unauthorized");
    }

    protected AuthenticateResult Success(string username)
    {
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

    protected string? validUser(string upwd)
    {
        string[] credentials = upwd.Split(':', 2);
        string username = credentials[0];
        string password = credentials[1];
        string[] users = File.ReadAllLines("users.txt");
        foreach (var user in users)
        {
            string[] udb = user.Split(';', 2);
            if (username == udb[0] && password == udb[1]) return username;
        }
        return null;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? uc = Request.Cookies["user"];
        string? username;

        if (uc != null)
        {
            string decrypted = Crypt.Decrypt(uc);
            username = validUser(decrypted);
            if (username != null) {
                logger.LogInformation("User {username} authenticated via cookie", username);
                return Success(username);
            }
        }   

        string? authheader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authheader)) return Fail();

        string encodedCredentials = authheader.Substring("Basic ".Length).Trim();
        byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
        string upwd = System.Text.Encoding.UTF8.GetString(credentialBytes);
        username = validUser(upwd);
        if (username == null) {
            logger.LogWarning("Failed login attempt");
            return Fail();
        }

        // Set the response cookie to log the user in
        string encrypted = Crypt.Encrypt(upwd);
        Response.Cookies.Append("user", encrypted, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        logger.LogInformation("User {username} authenticated via Basic Auth", username);
        return Success(username);
    }
}