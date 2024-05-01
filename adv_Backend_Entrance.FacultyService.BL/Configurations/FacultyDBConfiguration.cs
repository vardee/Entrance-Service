using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.FacultyService;
using adv_Backend_Entrance.FacultyService.BL.Services;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace adv_Backend_Entrance.FacultyService.BL.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddFacultyBlServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FacultyDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("FacultyDatabasePostgres")));
            services.AddSingleton<RedisDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDBContext(connectionString);
            });
            services.AddScoped<IFacultyService, FacultyInformationService>();
            services.AddScoped<IFacultyInteractionsService, FacultyInteractionsService>();
            services.AddSingleton<TokenHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

    }
}
