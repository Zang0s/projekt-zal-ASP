using Microsoft.EntityFrameworkCore;
using WebApp.Models.Movies;
using Microsoft.AspNetCore.Identity;
using WebApp.CustomIdentity;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddAuthentication("Identity.Application")
            .AddCookie("Identity.Application", options =>
            {
                options.Cookie.Name = "Identity.Application";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

        builder.Services.AddIdentityCore<CustomUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddSignInManager()
            .AddUserStore<CustomUserStore>()
            .AddDefaultTokenProviders();
        
        
        builder.Services.AddAuthorization();
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
        });

        var app = builder.Build();
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        

        app.Run();
    }
}