var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.UseStaticFiles(); 
app.MapControllerRoute(
    name: "Pronia",
    pattern:"{controller=home}/{action=index}/{id?}"
    ) ;

app.Run();
