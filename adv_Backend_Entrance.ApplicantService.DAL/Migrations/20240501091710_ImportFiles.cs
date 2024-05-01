using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.ApplicantService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ImportFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "educationDocumentImportFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_educationDocumentImportFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "passportImportFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passportImportFiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "educationDocumentImportFiles");

            migrationBuilder.DropTable(
                name: "passportImportFiles");
        }
    }
}
