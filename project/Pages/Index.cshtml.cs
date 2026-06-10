using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace project.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    private readonly IPersonConnector personConnector;
    public List<Person> plist { get; set;}

    public IndexModel(ILogger<IndexModel> logger, IPersonConnector personConnector)
    {
        this.logger = logger;
        this.personConnector = personConnector;
        plist = new();
    }
    public void OnGet()
    {
        List<Person>? list = personConnector.Get();
        if (list != null) {
            plist = list;
        }
    }
}
