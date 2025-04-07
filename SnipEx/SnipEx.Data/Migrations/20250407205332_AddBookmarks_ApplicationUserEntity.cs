using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnipEx.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookmarks_ApplicationUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Rating = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_ProgrammingLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "ProgrammingLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostsLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostsLikes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsTags",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostsTags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostsTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentsLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentsLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentsLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7"), "Content covering microservices design patterns, implementation strategies, service communication, and deployment considerations in distributed systems.", "Microservices-Architecture" },
                    { new Guid("31177cd3-13f4-44d2-b317-ab3f81aedc55"), "A powerful, easy-to-read programming language popular for web development, data science, AI, and scripting.", "Python" },
                    { new Guid("332c883e-7890-4286-9dd3-cb5ab81f4a81"), "A stylesheet language used for describing the presentation of a document written in HTML or XML.", "CSS" },
                    { new Guid("3be97903-3d50-4c29-ac9e-b2fdc84602ba"), "A strongly-typed superset of JavaScript that compiles to clean JavaScript code and improves maintainability in large projects.", "TypeScript" },
                    { new Guid("5bd996b0-ae55-4049-8159-7c513f3c8370"), "A class-based, object-oriented programming language designed to have as few implementation dependencies as possible, widely used in enterprise and mobile development.", "Java" },
                    { new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2"), "Topics related to implementing and managing dependency injection in .NET applications, including service lifetime management, container configuration, and best practices for loose coupling.", "Dependency-Injection" },
                    { new Guid("686a2ca8-742e-4659-833e-d7bbd9c7d310"), "A progressive JavaScript framework for building user interfaces and single-page applications.", "Vue" },
                    { new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac"), "Information about Entity Framework Core ORM, including performance optimization, query strategies, and database interaction patterns.", "Entity-Framework-Core" },
                    { new Guid("93692d51-2d0c-4a57-8c42-fee1be6fa3ba"), "A modern, object-oriented programming language developed by Microsoft, widely used for developing applications on the .NET platform.", "CSharp" },
                    { new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775"), "Discussions about Language Integrated Query (LINQ), including query optimization, collection handling, and advanced techniques for data manipulation in .NET applications.", "LINQ-and-Collections" },
                    { new Guid("af9a2024-9340-4451-84ad-b093ad05a7e1"), "A JavaScript library for building fast and interactive user interfaces, maintained by Facebook and a strong community.", "React" },
                    { new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93"), "Resources and discussions about implementing robust error handling, exception management, and logging strategies in .NET applications.", "Error-Handling-Patterns" },
                    { new Guid("d2501bb3-3806-435d-bde9-30ef5e3c759b"), "The standard markup language for creating web pages and web applications.", "HTML" },
                    { new Guid("ea5c4859-1ff2-4804-9d82-eb498397d6ba"), "A platform and framework for building single-page client applications using HTML and TypeScript, developed by Google.", "Angular" },
                    { new Guid("f8b2bf6d-bf81-44e7-912a-7670c92b5e18"), "A domain-specific language used in programming and managing relational databases.", "SQL" },
                    { new Guid("ff453f57-c9ad-4914-8e76-794b5b917ef8"), "A versatile, high-level programming language commonly used for web development, supporting both front-end and back-end applications.", "JavaScript" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "ApplicationUserId", "Content", "CreatedAt", "LanguageId", "Rating", "Title", "UserId", "Views" },
                values: new object[,]
                {
                    { new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), null, "Language Integrated Query (LINQ) is a powerful feature in C# that enables you to write type-safe queries. This comprehensive guide delves into advanced LINQ techniques, including query optimization, proper use of deferred execution, and understanding the performance implications of different LINQ operations. We'll examine real-world scenarios and demonstrate how to write efficient queries that scale well with large datasets.", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), 4.9m, "Mastering LINQ: Advanced Queries and Performance Optimization", null, 1000 },
                    { new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), null, "Entity Framework Core is the go-to ORM for .NET developers, but using it effectively requires understanding its inner workings. This post explores advanced patterns including query optimization, proper use of tracking vs. no-tracking queries, and effective caching strategies. We'll also examine common performance pitfalls and how to avoid them in real-world scenarios.", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), 4.6m, "Entity Framework Core: Advanced Patterns and Performance Tips", null, 912 },
                    { new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), null, "Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for resolving dependencies. In .NET Core, the built-in DI container provides a simple and flexible way to manage object dependencies. This post explores best practices, common pitfalls, and real-world examples of implementing DI in your applications. We'll cover constructor injection, service lifetimes, and how to properly structure your application for testability.", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), 4.8m, "Understanding Dependency Injection in .NET Core Applications", null, 856 },
                    { new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), null, "Proper error handling is crucial for building robust and maintainable applications. This guide covers advanced exception handling techniques in C#, including custom exceptions, global error handling middleware in ASP.NET Core, and logging best practices. Learn how to implement a comprehensive error handling strategy that improves application reliability and debugging capabilities.", new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), 4.7m, "Effective Error Handling in C# Applications", null, 645 },
                    { new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), null, "Microservices architecture has become increasingly popular for building scalable, maintainable applications. This post examines the key principles of microservices design using ASP.NET Core, including service communication patterns, data consistency challenges, and deployment strategies. We'll walk through creating a sample microservices ecosystem with practical examples and best practices.", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"), 4.5m, "Building Scalable Microservices with ASP.NET Core", null, 723 }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "ParentCommentId", "PostId", "UserId" },
                values: new object[,]
                {
                    { new Guid("43007dab-e354-4c2b-a2d2-b9c873c1c20e"), "The section about query optimization was eye-opening. I never realized how much performance impact the difference between IEnumerable and IQueryable could have. Would love to see a follow-up post about handling complex joins efficiently.", new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"), null },
                    { new Guid("6563ecc2-48e7-417b-b0fc-0e97bf1ef1dc"), "The microservices architecture patterns described here helped me understand how to better structure our distributed system. The examples of handling data consistency across services were particularly valuable.", new DateTime(2024, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"), null },
                    { new Guid("84cd3f43-760c-4a00-9f01-37c450c1b1d6"), "This article provided exactly what I needed to understand dependency injection properly. The examples were clear and the explanations of service lifetimes were particularly helpful. I've already started implementing these patterns in my current project.", new DateTime(2024, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"), null },
                    { new Guid("d80eb8f8-f40c-4118-844a-5bb6e272433d"), "The performance optimization techniques for Entity Framework Core were invaluable. I especially appreciated the detailed explanation of when to use different tracking options and how they affect application performance.", new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"), null },
                    { new Guid("fa286f96-7a7b-45ab-8e4b-4a34cc5ff75f"), "Great coverage of exception handling best practices. The middleware implementation example was especially useful. I've implemented similar patterns in my projects and it has significantly improved our error tracking.", new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"), null }
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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsLikes_CommentId_UserId",
                table: "CommentsLikes",
                columns: new[] { "CommentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentsLikes_UserId",
                table: "CommentsLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ActorId",
                table: "Notifications",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId_IsRead",
                table: "Notifications",
                columns: new[] { "RecipientId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ApplicationUserId",
                table: "Posts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_LanguageId",
                table: "Posts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsLikes_PostId_UserId",
                table: "PostsLikes",
                columns: new[] { "PostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostsLikes_UserId",
                table: "PostsLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsTags_TagId",
                table: "PostsTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CommentsLikes");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PostsLikes");

            migrationBuilder.DropTable(
                name: "PostsTags");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguages");
        }
    }
}
