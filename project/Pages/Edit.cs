using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
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

    public IActionResult OnPost(string btn)
    {
        Console.WriteLine($"Button {btn} was clicked");
        switch (btn)
        {
            case "save":
                if (p.Id == 0)
                {
                    Person np = personConnector.Create(p);
                    logger.LogInformation($"Created new person with id {np.Id}");
                }
                else
                {
                    bool ok = personConnector.Update(p);
                    logger.LogInformation($"Updated person with id {p.Id}: {ok}");
                }
                break;
            case "delete":
                if (p.Id != 0)
                {
                    bool ok = personConnector.Delete(p.Id);
                    logger.LogInformation($"Deleted person with id {p.Id}: {ok}");
                }
                break;
            case "cancel":
                logger.LogInformation("Edit cancelled");
                break;
        }
        return RedirectToPage("Index");
    }
}