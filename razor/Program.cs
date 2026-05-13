var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ITime, MyTime>();

var app = builder.Build();

// ===============================================

app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
