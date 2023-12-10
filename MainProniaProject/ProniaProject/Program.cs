using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Interfaces;
using ProniaProject.Models;
using ProniaProject.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    opt=>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric=false;
    options.Password.RequireDigit=true;
    options.Password.RequireLowercase=true;   
    options.Password.RequireUppercase=true;

    options.User.RequireUniqueEmail=true;

    options.Lockout.AllowedForNewUsers=true;
    options.Lockout.MaxFailedAccessAttempts=3;
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(3);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<FooterService>();
builder.Services.AddScoped<IEmailService,EmailService>();

var app = builder.Build();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();    
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
