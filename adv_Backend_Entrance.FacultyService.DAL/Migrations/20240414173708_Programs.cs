using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Programs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationProgrammModels_EducationLevelModels_EducationLevel~",
                table: "EducationProgrammModels");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationProgrammModels_FacultyModels_FacultyId",
                table: "EducationProgrammModels");

            migrationBuilder.DropIndex(
                name: "IX_EducationProgrammModels_EducationLevelId",
                table: "EducationProgrammModels");

            migrationBuilder.DropIndex(
                name: "IX_EducationProgrammModels_FacultyId",
                table: "EducationProgrammModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EducationProgrammModels_EducationLevelId",
                table: "EducationProgrammModels",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationProgrammModels_FacultyId",
                table: "EducationProgrammModels",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationProgrammModels_EducationLevelModels_EducationLevel~",
                table: "EducationProgrammModels",
                column: "EducationLevelId",
                principalTable: "EducationLevelModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationProgrammModels_FacultyModels_FacultyId",
                table: "EducationProgrammModels",
                column: "FacultyId",
                principalTable: "FacultyModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
