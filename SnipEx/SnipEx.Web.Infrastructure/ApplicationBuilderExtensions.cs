namespace SnipEx.Web.Infrastructure
{
    using System.Text;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using SnipEx.Common;

    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> SeedAdminAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            await serviceProvider.SeedAdminAsync();

            return app;
        }

        public static WebApplicationBuilder UseJwtAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JwtSettings.Issuer,
                    ValidAudience = JwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(JwtSettings.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Getting token from cookies (for regular API calls)
                        if (context.Request.Cookies.TryGetValue("JwtToken", out var cookieToken))
                        {
                            context.Token = cookieToken;
                            return Task.CompletedTask;
                        }

                        // Getting token from query string (for SignalR)
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            return builder;
        }
    }
}
