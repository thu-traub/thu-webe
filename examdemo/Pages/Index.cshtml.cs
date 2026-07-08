using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace examdemo.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
    public readonly IMyArray srv;

    public IndexModel(ILogger<IndexModel> logger, IMyArray myArray)
    {
        _logger = logger;
        srv = myArray;
    }

    public void OnGet()
    {
        _logger.LogInformation("OnGet called");
    }
}
