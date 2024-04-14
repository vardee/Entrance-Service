using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EducationLevels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelid",
                table: "EducationDocumentTypes");

            migrationBuilder.RenameColumn(
                name: "EducationLevelid",
                table: "EducationProgrammModels",
                newName: "EducationLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_EducationProgrammModels_EducationLevelid",
                table: "EducationProgrammModels",
                newName: "IX_EducationProgrammModels_EducationLevelId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "EducationLevelModels",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EducationLevelModels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EducationLevelid",
                table: "EducationDocumentTypes",
                newName: "EducationLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_EducationDocumentTypes_EducationLevelid",
                table: "EducationDocumentTypes",
                newName: "IX_EducationDocumentTypes_EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevelModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.RenameColumn(
                name: "EducationLevelId",
                table: "EducationProgrammModels",
                newName: "EducationLevelid");

            migrationBuilder.RenameIndex(
                name: "IX_EducationProgrammModels_EducationLevelId",
                table: "EducationProgrammModels",
                newName: "IX_EducationProgrammModels_EducationLevelid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EducationLevelModels",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EducationLevelModels",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EducationLevelId",
                table: "EducationDocumentTypes",
                newName: "EducationLevelid");

            migrationBuilder.RenameIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes",
                newName: "IX_EducationDocumentTypes_EducationLevelid");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelid",
                table: "EducationDocumentTypes",
                column: "EducationLevelid",
                principalTable: "EducationLevelModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
