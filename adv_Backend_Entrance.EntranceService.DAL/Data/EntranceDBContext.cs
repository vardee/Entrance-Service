using adv_Backend_Entrance.EntranceService.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data
{
    public class EntranceDBContext : DbContext 
    {
        public EntranceDBContext(DbContextOptions<EntranceDBContext> options) : base(options) { }
        public DbSet<ApplicationModel> Applications { get; set; }
        public DbSet<ApplicationProgram> ApplicationPrograms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationProgram>()
                .HasKey(ap => new { ap.ApplicationId, ap.ProgramId });
        }
    }
}
