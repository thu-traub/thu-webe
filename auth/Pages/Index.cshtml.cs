using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace auth.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public string MyUser { get; set; } = string.Empty;
    public IActionResult OnGet()
    {
        MyUser = User.Identity?.Name ?? "Unknown";
        return Page();
    }
}
