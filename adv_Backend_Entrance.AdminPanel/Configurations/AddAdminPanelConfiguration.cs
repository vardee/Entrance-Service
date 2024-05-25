using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.UserService;

namespace adv_Backend_Entrance.AdminPanel.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddAdminPanelConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RedisDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDBContext(connectionString);
            });
            services.AddSingleton<TokenHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
