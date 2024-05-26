using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Interfaces.NotificationService;
using adv_Backend_Entrance_Notification.BL.Services;
using adv_Backend_Entrance_Notification.DAL.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using YourNamespace;

namespace adv_Backend_Entrance.Notificantion.BL.Configuration
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddNotificationBlServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotificationDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("NotificationDataBase")));
            services.AddTransient<INotificationService, NotificationSendService>();
            services.AddSingleton<TokenHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

    }
}
