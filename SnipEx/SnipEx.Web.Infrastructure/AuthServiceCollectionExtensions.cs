namespace SnipEx.Web.Infrastructure
{
    using System.Text;

    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using SnipEx.Common;

    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
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

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddExternalAuthentication(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = config.GetValue<string>("Authentication:Google:ClientId")!;
                    options.ClientSecret = config.GetValue<string>("Authentication:Google:ClientSecret")!;
                })
                .AddGitHub(options =>
                {
                    options.ClientId = config.GetValue<string>("Authentication:GitHub:ClientId")!;
                    options.ClientSecret = config.GetValue<string>("Authentication:GitHub:ClientSecret")!;
                    options.Scope.Add("user:email");
                });
            return services;
        }
    }
}
