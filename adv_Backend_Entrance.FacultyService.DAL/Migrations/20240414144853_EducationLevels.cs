using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EducationLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducationLevelModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevelModels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FacultyModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationLevelid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypes_EducationLevelModels_EducationLevelid",
                        column: x => x.EducationLevelid,
                        principalTable: "EducationLevelModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationProgrammModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    EducationForm = table.Column<string>(type: "text", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevelid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationProgrammModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationProgrammModels_EducationLevelModels_EducationLevel~",
                        column: x => x.EducationLevelid,
                        principalTable: "EducationLevelModels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationProgrammModels_FacultyModels_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "FacultyModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypeEducationLevels_EducationLevelModelId",
                table: "EducationDocumentTypeEducationLevels",
                column: "EducationLevelModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_EducationLevelid",
                table: "EducationDocumentTypes",
                column: "EducationLevelid");

            migrationBuilder.CreateIndex(
                name: "IX_EducationProgrammModels_EducationLevelid",
                table: "EducationProgrammModels",
                column: "EducationLevelid");

            migrationBuilder.CreateIndex(
                name: "IX_EducationProgrammModels_FacultyId",
                table: "EducationProgrammModels",
                column: "FacultyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationDocumentTypeEducationLevels");

            migrationBuilder.DropTable(
                name: "EducationProgrammModels");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "FacultyModels");

            migrationBuilder.DropTable(
                name: "EducationLevelModels");
        }
    }
}
