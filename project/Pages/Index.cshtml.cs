using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace project.Pages;

public class IndexModel : PageModel
{
    public Person person { get; set; }
    public IndexModel()
    {
        person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Age = 30
        };
    }
    public void OnGet()
    {
     
    }
}
