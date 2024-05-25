using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.EntranceService;
using adv_Backend_Entrance.EntranceService.BL.Services;
using adv_Backend_Entrance.EntranceService.DAL.Data;
using EasyNetQ;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.BL.Configuration
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddEntranceDBConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EntranceDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("EntranceDBContext")));
            services.AddSingleton<RedisDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDBContext(connectionString);
            });
            services.AddHttpClient();
            services.AddScoped<IEntranceService, ApplicantEntranceService>();
            services.AddScoped<IManagerService, ManagerEntranceService>();
            services.AddScoped<ManagerHelperService>();
            services.AddSingleton<TokenHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

    }
}
