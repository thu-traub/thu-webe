using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examdemo.Pages;

public class IndexModel : PageModel
{
    public string[] Names { get; set; } = new string[] { "Alice", "Bob", "Charlie" };

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("OnGet called");
    }
}
