using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DocumentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationDocumentTypeEducationLevels");

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypeNextEducationLevels",
                columns: table => new
                {
                    EducationDocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypeNextEducationLevels", x => new { x.EducationDocumentTypeId, x.EducationLevelId });
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeNextEducationLevels_EducationDocumentT~",
                        column: x => x.EducationDocumentTypeId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeNextEducationLevels_EducationLevelMode~",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevelModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypeNextEducationLevels_EducationLevelId",
                table: "EducationDocumentTypeNextEducationLevels",
                column: "EducationLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationDocumentTypeNextEducationLevels");

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypeEducationLevels",
                columns: table => new
                {
                    EducationDocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevelModelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypeEducationLevels", x => new { x.EducationDocumentTypeId, x.EducationLevelModelId });
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeEducationLevels_EducationDocumentTypes~",
                        column: x => x.EducationDocumentTypeId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeEducationLevels_EducationLevelModels_E~",
                        column: x => x.EducationLevelModelId,
                        principalTable: "EducationLevelModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypeEducationLevels_EducationLevelModelId",
                table: "EducationDocumentTypeEducationLevels",
                column: "EducationLevelModelId");
        }
    }
}
