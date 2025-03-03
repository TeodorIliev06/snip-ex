using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProgrammingLanguageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LanguageId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ProgrammingLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingLanguages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"),
                column: "LanguageId",
                value: new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"),
                column: "LanguageId",
                value: new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"),
                column: "LanguageId",
                value: new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"),
                column: "LanguageId",
                value: new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"),
                column: "LanguageId",
                value: new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"));

            migrationBuilder.InsertData(
                table: "ProgrammingLanguages",
                columns: new[] { "Id", "FileExtension", "Name" },
                values: new object[,]
                {
                    { new Guid("a1e2c3d4-56b7-89f0-a1b2-c3d4e5f67890"), ".js", "JavaScript" },
                    { new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), ".cs", "C#" },
                    { new Guid("c3d4e5f6-7890-a1b2-c3d4-e5f67890abcd"), ".py", "Python" },
                    { new Guid("d4e5f678-90ab-c1d2-e3f4-567890abcdef"), ".java", "Java" },
                    { new Guid("e5f67890-abcd-1234-5678-90abcdef1234"), ".go", "Go" },
                    { new Guid("f67890ab-cdef-5678-90ab-cdef12345678"), ".ts", "TypeScript" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_LanguageId",
                table: "Posts",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ProgrammingLanguages_LanguageId",
                table: "Posts",
                column: "LanguageId",
                principalTable: "ProgrammingLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ProgrammingLanguages_LanguageId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Posts_LanguageId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Posts");
        }
    }
}
