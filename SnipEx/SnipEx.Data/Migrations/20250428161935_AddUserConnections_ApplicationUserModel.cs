using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserConnections_ApplicationUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersConnections",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersConnections", x => new { x.UserId, x.ConnectedUserId });
                    table.ForeignKey(
                        name: "FK_UsersConnections_AspNetUsers_ConnectedUserId",
                        column: x => x.ConnectedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsersConnections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersConnections_ConnectedUserId",
                table: "UsersConnections",
                column: "ConnectedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersConnections");
        }
    }
}
