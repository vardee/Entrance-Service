using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.UserService.BL.Services;
using adv_Backend_Entrance.UserService.DAL.Data;
using AutoMapper;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace adv_Backend_Entrance.UserService.BL.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddAuthBlServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AuthDatabasePostgres")));
            services.AddSingleton<RedisDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDBContext(connectionString);
            });

            services.AddScoped<IUserService, UsersService>();
            services.AddScoped<IManagerAccountService, ManagerAccountService>();
            services.AddScoped<AdditionTokenService>();
            services.AddSingleton<TokenHelper>();
            return services;
        }
    }
}
