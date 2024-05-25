using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace adv_Backend_Entrance.Common.JWT
{
    public static class JWTConfigurationService
    {
        public static IServiceCollection CreateJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? string.Empty)),
                    ValidateIssuerSigningKey = true,
                };
            });
            return services;
        }

        public static void AddAuthenticationWithSession(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationScheme = JwtBearerDefaults.AuthenticationScheme;
            var existingScheme = services.Any(s => s.ServiceType == typeof(IAuthenticationHandlerProvider) && s.ImplementationType.Name.Equals(authenticationScheme));

            if (!existingScheme)
            {
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = authenticationScheme;
                    x.DefaultChallengeScheme = authenticationScheme;
                }).AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Secret"])),
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateAudience = false
                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var session = context.HttpContext.Session;
                            var token = session.GetString("AccessToken");
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            }
        }
    }
}
