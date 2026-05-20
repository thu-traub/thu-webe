using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    public string Message { get; set; } = "";
    public int? cnt { get; set; }

    private readonly ITime timeService;

    public IndexModel(ILogger<IndexModel> logger, ITime timeService)
    {
        this.logger = logger;
        this.timeService = timeService;
    }
    public void OnGet(string id)
    {
        Message = "---";
    }

    public void OnPost()
    {
        cnt = HttpContext.Session.GetInt32("cnt");
        if (cnt == null) cnt = 0;
        Message = "#click = " + (++cnt);
        HttpContext.Session.SetInt32("cnt", cnt.Value);
    }

}

