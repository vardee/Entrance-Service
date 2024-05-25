using adv_Backend_Entrance.ApplicantService.DAL.Data.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.ApplicantService.DAL.Data
{
    public class ApplicantDBContext : DbContext
    {
        public ApplicantDBContext(DbContextOptions<ApplicantDBContext> options) : base(options) { }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<EducationDocument> EducationDocuments { get; set; }
        public DbSet<PassportImportFile> passportImportFiles { get; set; }
        public DbSet<EducationDocumentImportFile> educationDocumentImportFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
