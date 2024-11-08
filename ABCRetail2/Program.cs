using Microsoft.EntityFrameworkCore;
using ABCRetail2.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ABCRetail2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext with SQL Server connection string from appsettings.json
            builder.Services.AddDbContext<RetailContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ABCRetailDatabase")));

            // Add IHttpContextAccessor to access HttpContext in views and controllers
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add distributed memory cache and session support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use session middleware before authorization to ensure session data is available
            app.UseSession();

            app.UseAuthorization();

            // Map controller routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}
