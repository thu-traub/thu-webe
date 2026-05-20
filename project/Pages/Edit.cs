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
        logger.LogInformation("Editing person with id {id}", id);
    }

    public IActionResult OnPost()
    {
        bool ok = personConnector.Update(p);
        logger.LogInformation($"Updated person with id {p.Id}: {ok}");
        return RedirectToPage("Index");
    }
}