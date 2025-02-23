using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeAppUserNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "CreatedAt", "Rating", "Title", "UserId", "Views" },
                values: new object[,]
                {
                    { new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), "Language Integrated Query (LINQ) is a powerful feature in C# that enables you to write type-safe queries. This comprehensive guide delves into advanced LINQ techniques, including query optimization, proper use of deferred execution, and understanding the performance implications of different LINQ operations. We'll examine real-world scenarios and demonstrate how to write efficient queries that scale well with large datasets.", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), 4.9m, "Mastering LINQ: Advanced Queries and Performance Optimization", null, 1000 },
                    { new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), "Entity Framework Core is the go-to ORM for .NET developers, but using it effectively requires understanding its inner workings. This post explores advanced patterns including query optimization, proper use of tracking vs. no-tracking queries, and effective caching strategies. We'll also examine common performance pitfalls and how to avoid them in real-world scenarios.", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 4.6m, "Entity Framework Core: Advanced Patterns and Performance Tips", null, 912 },
                    { new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), "Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for resolving dependencies. In .NET Core, the built-in DI container provides a simple and flexible way to manage object dependencies. This post explores best practices, common pitfalls, and real-world examples of implementing DI in your applications. We'll cover constructor injection, service lifetimes, and how to properly structure your application for testability.", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 4.8m, "Understanding Dependency Injection in .NET Core Applications", null, 856 },
                    { new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), "Proper error handling is crucial for building robust and maintainable applications. This guide covers advanced exception handling techniques in C#, including custom exceptions, global error handling middleware in ASP.NET Core, and logging best practices. Learn how to implement a comprehensive error handling strategy that improves application reliability and debugging capabilities.", new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), 4.7m, "Effective Error Handling in C# Applications", null, 645 },
                    { new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), "Microservices architecture has become increasingly popular for building scalable, maintainable applications. This post examines the key principles of microservices design using ASP.NET Core, including service communication patterns, data consistency challenges, and deployment strategies. We'll walk through creating a sample microservices ecosystem with practical examples and best practices.", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4.5m, "Building Scalable Microservices with ASP.NET Core", null, 723 }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7"), "Content covering microservices design patterns, implementation strategies, service communication, and deployment considerations in distributed systems.", "Microservices-Architecture" },
                    { new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2"), "Topics related to implementing and managing dependency injection in .NET applications, including service lifetime management, container configuration, and best practices for loose coupling.", "Dependency-Injection" },
                    { new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac"), "Information about Entity Framework Core ORM, including performance optimization, query strategies, and database interaction patterns.", "Entity-Framework-Core" },
                    { new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775"), "Discussions about Language Integrated Query (LINQ), including query optimization, collection handling, and advanced techniques for data manipulation in .NET applications.", "LINQ-and-Collections" },
                    { new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93"), "Resources and discussions about implementing robust error handling, exception management, and logging strategies in .NET applications.", "Error-Handling-Patterns" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "PostId", "UserId" },
                values: new object[,]
                {
                    { new Guid("43007dab-e354-4c2b-a2d2-b9c873c1c20e"), "The section about query optimization was eye-opening. I never realized how much performance impact the difference between IEnumerable and IQueryable could have. Would love to see a follow-up post about handling complex joins efficiently.", new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), null },
                    { new Guid("6563ecc2-48e7-417b-b0fc-0e97bf1ef1dc"), "The microservices architecture patterns described here helped me understand how to better structure our distributed system. The examples of handling data consistency across services were particularly valuable.", new DateTime(2024, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), null },
                    { new Guid("84cd3f43-760c-4a00-9f01-37c450c1b1d6"), "This article provided exactly what I needed to understand dependency injection properly. The examples were clear and the explanations of service lifetimes were particularly helpful. I've already started implementing these patterns in my current project.", new DateTime(2024, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), null },
                    { new Guid("d80eb8f8-f40c-4118-844a-5bb6e272433d"), "The performance optimization techniques for Entity Framework Core were invaluable. I especially appreciated the detailed explanation of when to use different tracking options and how they affect application performance.", new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), null },
                    { new Guid("fa286f96-7a7b-45ab-8e4b-4a34cc5ff75f"), "Great coverage of exception handling best practices. The middleware implementation example was especially useful. I've implemented similar patterns in my projects and it has significantly improved our error tracking.", new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), null }
                });

            migrationBuilder.InsertData(
                table: "PostsTags",
                columns: new[] { "PostId", "TagId" },
                values: new object[,]
                {
                    { new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775") },
                    { new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac") },
                    { new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2") },
                    { new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93") },
                    { new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("43007dab-e354-4c2b-a2d2-b9c873c1c20e"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("6563ecc2-48e7-417b-b0fc-0e97bf1ef1dc"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("84cd3f43-760c-4a00-9f01-37c450c1b1d6"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("d80eb8f8-f40c-4118-844a-5bb6e272433d"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("fa286f96-7a7b-45ab-8e4b-4a34cc5ff75f"));

            migrationBuilder.DeleteData(
                table: "PostsTags",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775") });

            migrationBuilder.DeleteData(
                table: "PostsTags",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac") });

            migrationBuilder.DeleteData(
                table: "PostsTags",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2") });

            migrationBuilder.DeleteData(
                table: "PostsTags",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93") });

            migrationBuilder.DeleteData(
                table: "PostsTags",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7") });

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"));

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"));

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"));

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"));

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
