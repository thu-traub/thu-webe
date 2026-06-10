using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace auth.Pages;

public class IndexModel : PageModel
{
    public string MyUser { get; set; } = string.Empty;
    public IActionResult OnGet()
    {
        string? authheader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authheader))
        {
            HttpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
            return Unauthorized();
        }
        string encodedCredentials = authheader.Substring("Basic ".Length).Trim();
        byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
        string[] credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        string username = credentials[0];
        string password = credentials[1];
        if (password != "demo")
        {
            HttpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyRealm\"";
            return Unauthorized();
        }
        MyUser = username;
        return Page();
    }
}
