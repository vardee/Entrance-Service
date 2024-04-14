using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
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
            services.AddScoped<IFacultyService, FacultyInformationService>();
            services.AddSingleton<TokenHelper>();

            // Регистрация IHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

    }
}
