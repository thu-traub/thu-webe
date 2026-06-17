using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IPersonConnector, PersonListJson>();
builder.Services.AddAuthentication("mydemo").
AddScheme<AuthenticationSchemeOptions, AuthHandler>("mydemo", options => { });

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseApi();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
