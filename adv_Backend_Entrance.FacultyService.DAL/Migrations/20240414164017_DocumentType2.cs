using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DocumentType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevelModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
