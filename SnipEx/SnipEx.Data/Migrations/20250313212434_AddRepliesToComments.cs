using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRepliesToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("43007dab-e354-4c2b-a2d2-b9c873c1c20e"),
                column: "ParentCommentId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("6563ecc2-48e7-417b-b0fc-0e97bf1ef1dc"),
                column: "ParentCommentId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("84cd3f43-760c-4a00-9f01-37c450c1b1d6"),
                column: "ParentCommentId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("d80eb8f8-f40c-4118-844a-5bb6e272433d"),
                column: "ParentCommentId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("fa286f96-7a7b-45ab-8e4b-4a34cc5ff75f"),
                column: "ParentCommentId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");
        }
    }
}
