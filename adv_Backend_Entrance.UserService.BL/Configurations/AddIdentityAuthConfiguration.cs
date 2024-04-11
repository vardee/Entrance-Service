using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adv_Backend_Entrance.UserService.DAL.Data;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;

namespace adv_Backend_Entrance.UserService.BL.Configurations
{
    public static class IdentityDependenciesConfiguration
    {
        public static IServiceCollection AddIdentityDependenciesConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<User>>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>();
            return services;
        }
    }
}
