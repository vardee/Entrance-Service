using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.ApplicantService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ImportFiles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "EducationDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "EducationDocuments");
        }
    }
}
