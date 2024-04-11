using adv_Backend_Entrance.Common.Interfaces;
using adv_Backend_Entrance.Common.Models;
using adv_Backend_Entrance.UserService.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace adv_Backend_Entrance.UserService.DAL.Data
{
    public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IAuthDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<BlackToken> BlackListTokens { get; set; }
    }
}
