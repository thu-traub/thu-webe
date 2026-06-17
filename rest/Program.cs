var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/hello", () => new Person { Name = "World" });

app.Run();

public class Person
{
    public required string Name { get; set; }
}