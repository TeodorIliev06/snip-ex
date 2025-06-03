using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatingProperty_PostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Posts",
                type: "decimal(2,1)",
                precision: 2,
                scale: 1,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"),
                column: "Rating",
                value: 4.9m);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"),
                column: "Rating",
                value: 4.6m);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"),
                column: "Rating",
                value: 4.8m);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"),
                column: "Rating",
                value: 4.7m);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"),
                column: "Rating",
                value: 4.5m);
        }
    }
}
