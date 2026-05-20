using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    public string Message { get; set; }
    [BindProperty] public int cnt { get; set; }

    // [BindProperty]
    // public Test test { get; set; }
    private readonly ITime timeService;

    public IndexModel(ILogger<IndexModel> logger, ITime timeService)
    {
        this.logger = logger;
        this.timeService = timeService;
    }
    public void OnGet(string id)
    {
        Message = cnt.ToString();
    }

    public void OnPost()
    {
        Message = "#click = " + (++cnt);
        // string? c = HttpContext.Request.Cookies["cnt"];
        // if (c != null) cnt = Int32.Parse(c);
        // Message = "#click = " + (++cnt);
        // HttpContext.Response.Cookies.Append("cnt", cnt.ToString());
    }
    // public void OnPost(string button)
    // {
    //     logger.LogInformation($"Button {button} clicked at {DateTime.Now}");
    // }
    // public void OnPostEdit(string button)
    // {
    //     logger.LogInformation($"Edit: Button {button} clicked at {DateTime.Now}");
    // }
}

// public class Test
// {
//     public string name { get; set; } = "";
//     public string alter { get; set; } = "";
// }