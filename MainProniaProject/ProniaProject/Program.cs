using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    opt=>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );
builder.Services.AddScoped<FooterService>();
var app = builder.Build();
app.UseStaticFiles();
app.MapControllerRoute(
     name: "areas",
     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    ) ;

app.MapControllerRoute(
    name: "Pronia",
    pattern:"{controller=home}/{action=index}/{id?}"
    ) ;

app.Run();
