using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("31177cd3-13f4-44d2-b317-ab3f81aedc55"), "A powerful, easy-to-read programming language popular for web development, data science, AI, and scripting.", "Python" },
                    { new Guid("332c883e-7890-4286-9dd3-cb5ab81f4a81"), "A stylesheet language used for describing the presentation of a document written in HTML or XML.", "CSS" },
                    { new Guid("3be97903-3d50-4c29-ac9e-b2fdc84602ba"), "A strongly-typed superset of JavaScript that compiles to clean JavaScript code and improves maintainability in large projects.", "TypeScript" },
                    { new Guid("5bd996b0-ae55-4049-8159-7c513f3c8370"), "A class-based, object-oriented programming language designed to have as few implementation dependencies as possible, widely used in enterprise and mobile development.", "Java" },
                    { new Guid("686a2ca8-742e-4659-833e-d7bbd9c7d310"), "A progressive JavaScript framework for building user interfaces and single-page applications.", "Vue" },
                    { new Guid("93692d51-2d0c-4a57-8c42-fee1be6fa3ba"), "A modern, object-oriented programming language developed by Microsoft, widely used for developing applications on the .NET platform.", "CSharp" },
                    { new Guid("af9a2024-9340-4451-84ad-b093ad05a7e1"), "A JavaScript library for building fast and interactive user interfaces, maintained by Facebook and a strong community.", "React" },
                    { new Guid("d2501bb3-3806-435d-bde9-30ef5e3c759b"), "The standard markup language for creating web pages and web applications.", "HTML" },
                    { new Guid("ea5c4859-1ff2-4804-9d82-eb498397d6ba"), "A platform and framework for building single-page client applications using HTML and TypeScript, developed by Google.", "Angular" },
                    { new Guid("f8b2bf6d-bf81-44e7-912a-7670c92b5e18"), "A domain-specific language used in programming and managing relational databases.", "SQL" },
                    { new Guid("ff453f57-c9ad-4914-8e76-794b5b917ef8"), "A versatile, high-level programming language commonly used for web development, supporting both front-end and back-end applications.", "JavaScript" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("31177cd3-13f4-44d2-b317-ab3f81aedc55"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("332c883e-7890-4286-9dd3-cb5ab81f4a81"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("3be97903-3d50-4c29-ac9e-b2fdc84602ba"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("5bd996b0-ae55-4049-8159-7c513f3c8370"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("686a2ca8-742e-4659-833e-d7bbd9c7d310"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("93692d51-2d0c-4a57-8c42-fee1be6fa3ba"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("af9a2024-9340-4451-84ad-b093ad05a7e1"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("d2501bb3-3806-435d-bde9-30ef5e3c759b"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("ea5c4859-1ff2-4804-9d82-eb498397d6ba"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("f8b2bf6d-bf81-44e7-912a-7670c92b5e18"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("ff453f57-c9ad-4914-8e76-794b5b917ef8"));
        }
    }
}
