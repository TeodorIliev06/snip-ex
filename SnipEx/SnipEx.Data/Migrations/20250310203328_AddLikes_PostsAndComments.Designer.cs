﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SnipEx.Data;

#nullable disable

namespace SnipEx.Data.Migrations
{
    [DbContext(typeof(SnipExDbContext))]
    [Migration("20250310203328_AddLikes_PostsAndComments")]
    partial class AddLikes_PostsAndComments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SnipEx.Data.Models.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("SnipEx.Data.Models.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SnipEx.Data.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");

                    b.HasData(
                        new
                        {
                            Id = new Guid("84cd3f43-760c-4a00-9f01-37c450c1b1d6"),
                            Content = "This article provided exactly what I needed to understand dependency injection properly. The examples were clear and the explanations of service lifetimes were particularly helpful. I've already started implementing these patterns in my current project.",
                            CreatedAt = new DateTime(2024, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc),
                            PostId = new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb")
                        },
                        new
                        {
                            Id = new Guid("43007dab-e354-4c2b-a2d2-b9c873c1c20e"),
                            Content = "The section about query optimization was eye-opening. I never realized how much performance impact the difference between IEnumerable and IQueryable could have. Would love to see a follow-up post about handling complex joins efficiently.",
                            CreatedAt = new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                            PostId = new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8")
                        },
                        new
                        {
                            Id = new Guid("6563ecc2-48e7-417b-b0fc-0e97bf1ef1dc"),
                            Content = "The microservices architecture patterns described here helped me understand how to better structure our distributed system. The examples of handling data consistency across services were particularly valuable.",
                            CreatedAt = new DateTime(2024, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                            PostId = new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc")
                        },
                        new
                        {
                            Id = new Guid("fa286f96-7a7b-45ab-8e4b-4a34cc5ff75f"),
                            Content = "Great coverage of exception handling best practices. The middleware implementation example was especially useful. I've implemented similar patterns in my projects and it has significantly improved our error tracking.",
                            CreatedAt = new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc),
                            PostId = new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c")
                        },
                        new
                        {
                            Id = new Guid("d80eb8f8-f40c-4118-844a-5bb6e272433d"),
                            Content = "The performance optimization techniques for Entity Framework Core were invaluable. I especially appreciated the detailed explanation of when to use different tracking options and how they affect application performance.",
                            CreatedAt = new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc),
                            PostId = new Guid("5a250d42-f366-437a-9f21-c00f2d56e898")
                        });
                });

            modelBuilder.Entity("SnipEx.Data.Models.CommentLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("CommentId", "UserId")
                        .IsUnique();

                    b.ToTable("CommentsLikes");
                });

            modelBuilder.Entity("SnipEx.Data.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LanguageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Rating")
                        .HasPrecision(2, 1)
                        .HasColumnType("decimal(2,1)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Views")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");

                    b.HasData(
                        new
                        {
                            Id = new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"),
                            Content = "Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for resolving dependencies. In .NET Core, the built-in DI container provides a simple and flexible way to manage object dependencies. This post explores best practices, common pitfalls, and real-world examples of implementing DI in your applications. We'll cover constructor injection, service lifetimes, and how to properly structure your application for testability.",
                            CreatedAt = new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                            LanguageId = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            Rating = 4.8m,
                            Title = "Understanding Dependency Injection in .NET Core Applications",
                            Views = 856
                        },
                        new
                        {
                            Id = new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"),
                            Content = "Language Integrated Query (LINQ) is a powerful feature in C# that enables you to write type-safe queries. This comprehensive guide delves into advanced LINQ techniques, including query optimization, proper use of deferred execution, and understanding the performance implications of different LINQ operations. We'll examine real-world scenarios and demonstrate how to write efficient queries that scale well with large datasets.",
                            CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            LanguageId = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            Rating = 4.9m,
                            Title = "Mastering LINQ: Advanced Queries and Performance Optimization",
                            Views = 1000
                        },
                        new
                        {
                            Id = new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"),
                            Content = "Microservices architecture has become increasingly popular for building scalable, maintainable applications. This post examines the key principles of microservices design using ASP.NET Core, including service communication patterns, data consistency challenges, and deployment strategies. We'll walk through creating a sample microservices ecosystem with practical examples and best practices.",
                            CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LanguageId = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            Rating = 4.5m,
                            Title = "Building Scalable Microservices with ASP.NET Core",
                            Views = 723
                        },
                        new
                        {
                            Id = new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"),
                            Content = "Proper error handling is crucial for building robust and maintainable applications. This guide covers advanced exception handling techniques in C#, including custom exceptions, global error handling middleware in ASP.NET Core, and logging best practices. Learn how to implement a comprehensive error handling strategy that improves application reliability and debugging capabilities.",
                            CreatedAt = new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc),
                            LanguageId = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            Rating = 4.7m,
                            Title = "Effective Error Handling in C# Applications",
                            Views = 645
                        },
                        new
                        {
                            Id = new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"),
                            Content = "Entity Framework Core is the go-to ORM for .NET developers, but using it effectively requires understanding its inner workings. This post explores advanced patterns including query optimization, proper use of tracking vs. no-tracking queries, and effective caching strategies. We'll also examine common performance pitfalls and how to avoid them in real-world scenarios.",
                            CreatedAt = new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc),
                            LanguageId = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            Rating = 4.6m,
                            Title = "Entity Framework Core: Advanced Patterns and Performance Tips",
                            Views = 912
                        });
                });

            modelBuilder.Entity("SnipEx.Data.Models.PostLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("PostId", "UserId")
                        .IsUnique();

                    b.ToTable("PostsLikes");
                });

            modelBuilder.Entity("SnipEx.Data.Models.PostTag", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PostId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("PostsTags");

                    b.HasData(
                        new
                        {
                            PostId = new Guid("606585ff-5f22-49a8-bb12-d1b52f155cfb"),
                            TagId = new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2")
                        },
                        new
                        {
                            PostId = new Guid("2deb2f40-7a55-4a0b-86d0-e3983cc460b8"),
                            TagId = new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775")
                        },
                        new
                        {
                            PostId = new Guid("8ad01fc6-828e-4e4a-aae8-efb860d210fc"),
                            TagId = new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7")
                        },
                        new
                        {
                            PostId = new Guid("714f4d63-8e48-4a31-abaf-e3142420a34c"),
                            TagId = new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93")
                        },
                        new
                        {
                            PostId = new Guid("5a250d42-f366-437a-9f21-c00f2d56e898"),
                            TagId = new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac")
                        });
                });

            modelBuilder.Entity("SnipEx.Data.Models.ProgrammingLanguage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("ProgrammingLanguages");

                    b.HasData(
                        new
                        {
                            Id = new Guid("b3f7e6d5-89f4-4b29-bd0e-31a3f9076b2d"),
                            FileExtension = ".cs",
                            Name = "C#"
                        },
                        new
                        {
                            Id = new Guid("a1e2c3d4-56b7-89f0-a1b2-c3d4e5f67890"),
                            FileExtension = ".js",
                            Name = "JavaScript"
                        },
                        new
                        {
                            Id = new Guid("c3d4e5f6-7890-a1b2-c3d4-e5f67890abcd"),
                            FileExtension = ".py",
                            Name = "Python"
                        },
                        new
                        {
                            Id = new Guid("d4e5f678-90ab-c1d2-e3f4-567890abcdef"),
                            FileExtension = ".java",
                            Name = "Java"
                        },
                        new
                        {
                            Id = new Guid("e5f67890-abcd-1234-5678-90abcdef1234"),
                            FileExtension = ".go",
                            Name = "Go"
                        },
                        new
                        {
                            Id = new Guid("f67890ab-cdef-5678-90ab-cdef12345678"),
                            FileExtension = ".ts",
                            Name = "TypeScript"
                        });
                });

            modelBuilder.Entity("SnipEx.Data.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5f064346-c5ee-4c85-a9d3-141f681953f2"),
                            Description = "Topics related to implementing and managing dependency injection in .NET applications, including service lifetime management, container configuration, and best practices for loose coupling.",
                            Name = "Dependency-Injection"
                        },
                        new
                        {
                            Id = new Guid("a6e14c7b-5321-463d-bdd9-6718ee40d775"),
                            Description = "Discussions about Language Integrated Query (LINQ), including query optimization, collection handling, and advanced techniques for data manipulation in .NET applications.",
                            Name = "LINQ-and-Collections"
                        },
                        new
                        {
                            Id = new Guid("2b8a891c-e627-40bd-8cdd-bfd0655f0fc7"),
                            Description = "Content covering microservices design patterns, implementation strategies, service communication, and deployment considerations in distributed systems.",
                            Name = "Microservices-Architecture"
                        },
                        new
                        {
                            Id = new Guid("b09d09f3-a249-4f48-841f-eecd5fd3ec93"),
                            Description = "Resources and discussions about implementing robust error handling, exception management, and logging strategies in .NET applications.",
                            Name = "Error-Handling-Patterns"
                        },
                        new
                        {
                            Id = new Guid("8ab0d353-2dcf-47e9-af14-9c8a66efa2ac"),
                            Description = "Information about Entity Framework Core ORM, including performance optimization, query strategies, and database interaction patterns.",
                            Name = "Entity-Framework-Core"
                        },
                        new
                        {
                            Id = new Guid("ff453f57-c9ad-4914-8e76-794b5b917ef8"),
                            Description = "A versatile, high-level programming language commonly used for web development, supporting both front-end and back-end applications.",
                            Name = "JavaScript"
                        },
                        new
                        {
                            Id = new Guid("31177cd3-13f4-44d2-b317-ab3f81aedc55"),
                            Description = "A powerful, easy-to-read programming language popular for web development, data science, AI, and scripting.",
                            Name = "Python"
                        },
                        new
                        {
                            Id = new Guid("af9a2024-9340-4451-84ad-b093ad05a7e1"),
                            Description = "A JavaScript library for building fast and interactive user interfaces, maintained by Facebook and a strong community.",
                            Name = "React"
                        },
                        new
                        {
                            Id = new Guid("93692d51-2d0c-4a57-8c42-fee1be6fa3ba"),
                            Description = "A modern, object-oriented programming language developed by Microsoft, widely used for developing applications on the .NET platform.",
                            Name = "CSharp"
                        },
                        new
                        {
                            Id = new Guid("3be97903-3d50-4c29-ac9e-b2fdc84602ba"),
                            Description = "A strongly-typed superset of JavaScript that compiles to clean JavaScript code and improves maintainability in large projects.",
                            Name = "TypeScript"
                        },
                        new
                        {
                            Id = new Guid("5bd996b0-ae55-4049-8159-7c513f3c8370"),
                            Description = "A class-based, object-oriented programming language designed to have as few implementation dependencies as possible, widely used in enterprise and mobile development.",
                            Name = "Java"
                        },
                        new
                        {
                            Id = new Guid("d2501bb3-3806-435d-bde9-30ef5e3c759b"),
                            Description = "The standard markup language for creating web pages and web applications.",
                            Name = "HTML"
                        },
                        new
                        {
                            Id = new Guid("332c883e-7890-4286-9dd3-cb5ab81f4a81"),
                            Description = "A stylesheet language used for describing the presentation of a document written in HTML or XML.",
                            Name = "CSS"
                        },
                        new
                        {
                            Id = new Guid("ea5c4859-1ff2-4804-9d82-eb498397d6ba"),
                            Description = "A platform and framework for building single-page client applications using HTML and TypeScript, developed by Google.",
                            Name = "Angular"
                        },
                        new
                        {
                            Id = new Guid("686a2ca8-742e-4659-833e-d7bbd9c7d310"),
                            Description = "A progressive JavaScript framework for building user interfaces and single-page applications.",
                            Name = "Vue"
                        },
                        new
                        {
                            Id = new Guid("f8b2bf6d-bf81-44e7-912a-7670c92b5e18"),
                            Description = "A domain-specific language used in programming and managing relational databases.",
                            Name = "SQL"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SnipEx.Data.Models.Comment", b =>
                {
                    b.HasOne("SnipEx.Data.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.ApplicationUser", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SnipEx.Data.Models.CommentLike", b =>
                {
                    b.HasOne("SnipEx.Data.Models.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.ApplicationUser", "User")
                        .WithMany("LikedComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SnipEx.Data.Models.Post", b =>
                {
                    b.HasOne("SnipEx.Data.Models.ProgrammingLanguage", "Language")
                        .WithMany("Posts")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.ApplicationUser", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Language");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SnipEx.Data.Models.PostLike", b =>
                {
                    b.HasOne("SnipEx.Data.Models.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.ApplicationUser", "User")
                        .WithMany("LikedPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SnipEx.Data.Models.PostTag", b =>
                {
                    b.HasOne("SnipEx.Data.Models.Post", "Post")
                        .WithMany("PostsTags")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SnipEx.Data.Models.Tag", "Tag")
                        .WithMany("PostsTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("SnipEx.Data.Models.ApplicationUser", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("LikedComments");

                    b.Navigation("LikedPosts");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("SnipEx.Data.Models.Comment", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("SnipEx.Data.Models.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("PostsTags");
                });

            modelBuilder.Entity("SnipEx.Data.Models.ProgrammingLanguage", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("SnipEx.Data.Models.Tag", b =>
                {
                    b.Navigation("PostsTags");
                });
#pragma warning restore 612, 618
        }
    }
}
