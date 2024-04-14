using adv_Backend_Entrance.FacultyService.DAL.Data.Models;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace adv_Backend_Entrance.FacultyService.MVCPanel.Data
{
    public class FacultyDBContext : DbContext
    {
        public FacultyDBContext(DbContextOptions<FacultyDBContext> options) : base(options) { }

        public DbSet<EducationLevelModel> EducationLevelModels { get; set; }
        public DbSet<EducationProgrammModel> EducationProgrammModels { get; set; }
        public DbSet<FacultyModel> FacultyModels { get; set; }
        public DbSet<EducationDocumentTypeModel> EducationDocumentTypes { get; set; }

        public DbSet<EducationDocumentTypeNextEducationLevel> EducationDocumentTypeNextEducationLevels { get; set; }

        public DbSet<Import> Imports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EducationDocumentTypeNextEducationLevel>()
                .HasKey(x => new { x.EducationDocumentTypeId, x.EducationLevelId });

            modelBuilder.Entity<EducationDocumentTypeNextEducationLevel>()
                .HasOne(x => x.EducationDocumentType)
                .WithMany()
                .HasForeignKey(x => x.EducationDocumentTypeId);

            modelBuilder.Entity<EducationDocumentTypeNextEducationLevel>()
                .HasOne(x => x.EducationLevel)
                .WithMany()
                .HasForeignKey(x => x.EducationLevelId);
            modelBuilder.Entity<EducationLevelModel>().HasKey(x => x.Id);
            modelBuilder.Entity<EducationLevelModel>().Property(x => x.Id).ValueGeneratedNever();
        }
    }
}
