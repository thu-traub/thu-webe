using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    public string Message { get; set; } = "";
    private readonly ITime timeService;

    public IndexModel(ILogger<IndexModel> logger, ITime timeService)
    {
        this.logger = logger;
        this.timeService = timeService;
    }
    public void OnGet()
    {
        logger.LogInformation("Index page visited at {Time}", DateTime.Now);
        Message = timeService.GetTime();
    }
}
