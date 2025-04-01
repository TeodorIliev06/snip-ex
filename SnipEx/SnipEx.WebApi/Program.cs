namespace SnipEx.WebApi
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data;
    using SnipEx.Data.Models;
    using SnipEx.WebApi.Hubs;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.Infrastructure;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Mediator.Comments.CommentAdded;
    using static SnipEx.Common.SignalRConstants;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SnipEx.Web", "wwwroot"))
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var appOrigin = builder.Configuration.GetValue<string>("ClientOrigins:SnipEx");
            // Add services to the container.

            builder.Services
                .AddDbContext<SnipExDbContext>(cfg =>
                {
                    cfg.UseSqlServer(connectionString);
                });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(cfg =>
                {
                    IdentityOptionsConfigurator.Configure(builder, cfg);
                })
                .AddEntityFrameworkStores<SnipExDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(IPostService).Assembly);

            builder.Services.AddSignalR();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CommentAddedEventHandler).Assembly));
            builder.UseJwtAuthentication();

            builder.Services.AddCors(cfg =>
            {
                cfg.AddPolicy("AllowAll", policyBld =>
                {
                    policyBld
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });

                if (!string.IsNullOrWhiteSpace(appOrigin))
                {
                    cfg.AddPolicy("AllowMyServer", policyBld =>
                    {
                        policyBld
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins(appOrigin);
                    });
                }
            });

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(PostCardViewModel).Assembly);

            app.MapHub<NotificationHub>(HubRoutes.Notifications);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!string.IsNullOrWhiteSpace(appOrigin))
            {
                app.UseCors("AllowMyServer");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
