using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace project.Pages;

public class IndexModel : PageModel
{
    public List<Person> plist { get; set; }
    public IndexModel()
    {
        plist = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 25 },
            new Person { Id = 3, FirstName = "Bob", LastName = "Johnson", Age = 40 }
        };
    }
    public void OnGet()
    {
     
    }
}
