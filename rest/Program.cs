var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

List<Person> DemoPerson = new List<Person> { 
    new Person { Id=1,Name = "John" },
    new Person { Id=2, Name = "Jane" }
};

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.File("index.html", "text/html"));

app.MapGet("/api/person", () => DemoPerson);
app.MapGet("/api/person/{id}", (int id) => DemoPerson.FirstOrDefault(p => p.Id == id));
app.MapPost("/api/person", (Person person) =>
{
    DemoPerson.Add(person);
    return Results.Ok(person);
}); 
app.MapPut("/api/person/{id}", (int id, Person updatedPerson) =>
{
    var person = DemoPerson.FirstOrDefault(p => p.Id == id);
    if (person is null)
    {
        return Results.NotFound();
    }
    person.Name = updatedPerson.Name;
    return Results.Ok(person);
});

app.Run();

public class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
}