namespace SnipEx.WebApi
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.Infrastructure;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add services to the container.

            builder.Services
                .AddDbContext<SnipExDbContext>(cfg =>
                {
                    cfg.UseSqlServer(connectionString);
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(IPostService).Assembly);

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(PostCardViewModel).Assembly);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
