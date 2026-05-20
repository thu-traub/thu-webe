using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EditModel : PageModel
{
    [BindProperty]
    public Person p { get; set; }

    private readonly ILogger<EditModel> logger;
    private readonly IPersonConnector personConnector;

    public EditModel(ILogger<EditModel> logger, IPersonConnector personConnector)
    {
        this.logger = logger;
        this.personConnector = personConnector;
        p = new();
    }
    public void OnGet(int id)
    {
        p = personConnector.Get(id) ?? new Person();
    }

    public void OnPost()
    {
        // Handle form submission and update the person details
    }
}