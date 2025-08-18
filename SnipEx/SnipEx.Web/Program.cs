namespace SnipEx.Web
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data;
    using SnipEx.Realtime;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Services.Mediator;
    using SnipEx.Web.Infrastructure;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

            // Add services to the container.
            builder.Services.AddDbContext<SnipExDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(cfg =>
                {
                    IdentityOptionsConfigurator.Configure(builder, cfg);
                })
                .AddEntityFrameworkStores<SnipExDbContext>()
                .AddRoles<ApplicationRole>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddUserManager<UserManager<ApplicationUser>>();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Error/403";
            });

            //Register SignalR before using mediator
            builder.Services.AddRealtimeServices();
            builder.Services.AddMediator();

            builder.Services.AddExternalAuthentication(builder.Configuration);

            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(IPostService).Assembly);

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(PostViewModel).Assembly);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error/404");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseAuthentication();
            app.UseAuthorization();

            await app.SeedAdminAsync();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}
