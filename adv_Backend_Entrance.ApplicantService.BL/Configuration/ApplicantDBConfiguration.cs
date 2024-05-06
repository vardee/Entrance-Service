using adv_Backend_Entrance.ApplicantService.BL.Helpers;
using adv_Backend_Entrance.ApplicantService.BL.Services;
using adv_Backend_Entrance.ApplicantService.DAL.Data;
using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.BL.Configuration
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddApplicantDBConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicantDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ApplicantDBContext")));
            services.AddSingleton<RedisDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDBContext(connectionString);
            });
            services.AddScoped<IApplicantService, ApplicantDocumentService>();
            services.AddScoped<IApplicantDocumentsFiles, ApplicantFilesService>();
            services.AddSingleton<TokenHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

    }
}
