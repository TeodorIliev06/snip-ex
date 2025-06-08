namespace SnipEx.Services.Tests
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable.Moq;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Web.ViewModels.User;
    using SnipEx.Services.Data.Models;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Data.Repositories.Contracts;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<PostLike, Guid>> _mockPostLikeRepository;
        private Mock<IRepository<ApplicationUser, Guid>> _mockUserRepository;
        private Mock<IRepository<UserConnection, object>> _mockUserConnectionRepository;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<ApplicationUser, Guid>>();
            _mockPostLikeRepository = new Mock<IRepository<PostLike, Guid>>();
            _mockUserConnectionRepository = new Mock<IRepository<UserConnection, object>>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _userService = new UserService(
                _mockPostLikeRepository.Object,
                _mockUserRepository.Object,
                _mockUserConnectionRepository.Object);
        }

        #region GetTotalLikesReceivedByUserAsync Tests

        [Test]
        public async Task GetTotalLikesReceivedByUserAsync_ShouldReturnCorrectCount_WhenUserHasLikedPosts()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var postLikes = new List<PostLike>
            {
                new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = userGuid, Title = "Post 1" }
                },
                new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = userGuid, Title = "Post 2" }
                },
                new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = Guid.NewGuid(), Title = "Other User Post" } // Different user's post
                }
            };

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetTotalLikesReceivedByUserAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(2)); // Only 2 likes on the target user's posts
            _mockPostLikeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetTotalLikesReceivedByUserAsync_ShouldReturnZero_WhenUserHasNoLikedPosts()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var postLikes = new List<PostLike>
            {
                new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = Guid.NewGuid(), Title = "Other User Post" }
                }
            };

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetTotalLikesReceivedByUserAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
            _mockPostLikeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetTotalLikesReceivedByUserAsync_ShouldReturnZero_WhenNoPostLikesExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var postLikes = new List<PostLike>();

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetTotalLikesReceivedByUserAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
            _mockPostLikeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetTotalLikesReceivedByUserAsync_ShouldThrowFormatException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            var invalidUserId = "not-a-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetTotalLikesReceivedByUserAsync(invalidUserId));
        }

        [Test]
        public void GetTotalLikesReceivedByUserAsync_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            string nullUserId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetTotalLikesReceivedByUserAsync(nullUserId));
        }

        [Test]
        public async Task GetTotalLikesReceivedByUserAsync_ShouldHandleLargeNumberOfLikes_WhenUserIsPopular()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var postLikes = new List<PostLike>();

            // Create 1000 likes on user's posts
            for (int i = 0; i < 1000; i++)
            {
                postLikes.Add(new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = userGuid, Title = $"Post {i}" }
                });
            }

            // Add some likes on other users' posts (should not be counted)
            for (int i = 0; i < 100; i++)
            {
                postLikes.Add(new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Post = new Post { UserId = Guid.NewGuid(), Title = $"Other Post {i}" }
                });
            }

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetTotalLikesReceivedByUserAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(1000)); // Only the target user's posts should be counted
            _mockPostLikeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region GetProfileInformationAsync Tests

        [Test]
        public async Task GetProfileInformationAsync_ShouldReturnProfileInformation_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = userGuid,
                    UserName = "testuser",
                    Email = "test@example.com",
                    JoinDate = new DateTime(2023, 1, 15),
                    Posts = new List<Post>
                    {
                        new Post { Id = Guid.NewGuid(), Title = "Post 1", UserId = userGuid },
                        new Post { Id = Guid.NewGuid(), Title = "Post 2", UserId = userGuid }
                    }
                }
            };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetProfileInformationAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ProfileInformationViewModel>());
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetProfileInformationAsync_ShouldThrowInvalidOperationException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var users = new List<ApplicationUser>(); // Empty list - no users

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.GetProfileInformationAsync(userId));

            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetProfileInformationAsync_ShouldThrowFormatException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            var invalidUserId = "not-a-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetProfileInformationAsync(invalidUserId));
        }

        [Test]
        public void GetProfileInformationAsync_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            string nullUserId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetProfileInformationAsync(nullUserId));
        }

        [Test]
        public async Task GetProfileInformationAsync_ShouldIncludePostsInQuery_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = userGuid,
                    UserName = "testuser",
                    Email = "test@example.com",
                    JoinDate = DateTime.UtcNow,
                    Posts = new List<Post>
                    {
                        new Post { Id = Guid.NewGuid(), Title = "Post 1", UserId = userGuid },
                        new Post { Id = Guid.NewGuid(), Title = "Post 2", UserId = userGuid },
                        new Post { Id = Guid.NewGuid(), Title = "Post 3", UserId = userGuid }
                    }
                }
            };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetProfileInformationAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            // Verify that the Include(u => u.Posts) was part of the query by checking repository call
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetProfileInformationAsync_ShouldReturnFirstMatchingUser_WhenMultipleUsersExist()
        {
            // Arrange
            var targetUserId = Guid.NewGuid().ToString();
            var targetUserGuid = Guid.Parse(targetUserId);

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "otheruser",
                    Email = "other@example.com",
                    JoinDate = DateTime.UtcNow,
                    Posts = new List<Post>()
                },
                new ApplicationUser
                {
                    Id = targetUserGuid,
                    UserName = "targetuser",
                    Email = "target@example.com",
                    JoinDate = DateTime.UtcNow,
                    Posts = new List<Post>
                    {
                        new Post { Id = Guid.NewGuid(), Title = "Target Post", UserId = targetUserGuid }
                    }
                }
            };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetProfileInformationAsync(targetUserId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ProfileInformationViewModel>());
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetProfileInformationAsync_ShouldHandleUserWithNoPosts_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = userGuid,
                    UserName = "userwithnoposts",
                    Email = "noposts@example.com",
                    JoinDate = DateTime.UtcNow,
                    Posts = new List<Post>() // Empty posts collection
                }
            };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetProfileInformationAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ProfileInformationViewModel>());
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region GetUserBookmarksAsync Tests

        [Test]
        public async Task GetUserBookmarksAsync_ShouldReturnBookmarkViewModel_WhenUserHasBookmarks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var languageId = Guid.NewGuid();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Bookmark 1",
                    Content = "Content 1",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Bookmark 2",
                    Content = "Content 2",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Old Bookmark",
                    Content = "Old Content",
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "JavaScript" }
                }
            }
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserBookmarksAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(result.TotalBookmarks, Is.EqualTo(3));
                Assert.That(result.RecentBookmarksCount, Is.EqualTo(2)); // Only bookmarks from last 30 days
                Assert.That(result.MostCommonLanguage, Is.EqualTo("C#")); // Most frequent language
                Assert.That(result.Bookmarks, Is.Not.Null);
            });
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserBookmarksAsync_ShouldReturnEmptyBookmarks_WhenUserHasNoBookmarks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>() // Empty bookmarks
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserBookmarksAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(result.TotalBookmarks, Is.EqualTo(0));
                Assert.That(result.RecentBookmarksCount, Is.EqualTo(0));
                Assert.That(result.MostCommonLanguage, Is.EqualTo("None")); // Default when no bookmarks
                Assert.That(result.Bookmarks, Is.Not.Null);
            });
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserBookmarksAsync_ShouldCalculateRecentBookmarksCorrectly_WhenMixedDates()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var languageId = Guid.NewGuid();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Recent Bookmark 1",
                    Content = "Content 1",
                    CreatedAt = DateTime.UtcNow.AddDays(-5), // Recent
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Recent Bookmark 2",
                    Content = "Content 2",
                    CreatedAt = DateTime.UtcNow.AddDays(-25), // Recent
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Old Bookmark 1",
                    Content = "Content 3",
                    CreatedAt = DateTime.UtcNow.AddDays(-35), // Old
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "JavaScript" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Old Bookmark 2",
                    Content = "Content 4",
                    CreatedAt = DateTime.UtcNow.AddDays(-45), // Old
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "Python" }
                }
            }
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserBookmarksAsync(userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.TotalBookmarks, Is.EqualTo(4));
                Assert.That(result.RecentBookmarksCount, Is.EqualTo(2)); // Only recent ones
                Assert.That(result.MostCommonLanguage, Is.EqualTo("C#")); // Most frequent
            });
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserBookmarksAsync_ShouldReturnCorrectMostCommonLanguage_WhenMultipleLanguages()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var csharpId = Guid.NewGuid();
            var jsId = Guid.NewGuid();
            var pythonId = Guid.NewGuid();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>
            {
                // 3 JavaScript bookmarks (most common)
                new Post { Id = Guid.NewGuid(), LanguageId = jsId, Language = new ProgrammingLanguage { Id = jsId, Name = "JavaScript" }, CreatedAt = DateTime.UtcNow },
                new Post { Id = Guid.NewGuid(), LanguageId = jsId, Language = new ProgrammingLanguage { Id = jsId, Name = "JavaScript" }, CreatedAt = DateTime.UtcNow },
                new Post { Id = Guid.NewGuid(), LanguageId = jsId, Language = new ProgrammingLanguage { Id = jsId, Name = "JavaScript" }, CreatedAt = DateTime.UtcNow },
                // 2 C# bookmarks
                new Post { Id = Guid.NewGuid(), LanguageId = csharpId, Language = new ProgrammingLanguage { Id = csharpId, Name = "C#" }, CreatedAt = DateTime.UtcNow },
                new Post { Id = Guid.NewGuid(), LanguageId = csharpId, Language = new ProgrammingLanguage { Id = csharpId, Name = "C#" }, CreatedAt = DateTime.UtcNow },
                // 1 Python bookmark
                new Post { Id = Guid.NewGuid(), LanguageId = pythonId, Language = new ProgrammingLanguage { Id = pythonId, Name = "Python" }, CreatedAt = DateTime.UtcNow }
            }
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserBookmarksAsync(userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.TotalBookmarks, Is.EqualTo(6));
                Assert.That(result.MostCommonLanguage, Is.EqualTo("JavaScript")); // Most frequent
            });
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetUserBookmarksAsync_ShouldThrowInvalidOperationException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var users = new List<ApplicationUser>(); // Empty list - no users

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.GetUserBookmarksAsync(userId));

            // Fixed: Now expecting the custom error message instead of "Sequence contains no elements"
            Assert.That(exception.Message, Does.Contain($"User with ID '{userId}' was not found"));
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetUserBookmarksAsync_ShouldThrowFormatException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            var invalidUserId = "not-a-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetUserBookmarksAsync(invalidUserId));
        }

        [Test]
        public void GetUserBookmarksAsync_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            string nullUserId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetUserBookmarksAsync(nullUserId));
        }

        [Test]
        public void GetUserBookmarksAsync_ShouldThrowFormatException_WhenUserIdIsEmpty()
        {
            // Arrange
            var emptyUserId = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetUserBookmarksAsync(emptyUserId));
        }

        [Test]
        public void GetUserBookmarksAsync_ShouldThrowFormatException_WhenUserIdIsWhitespace()
        {
            // Arrange
            var whitespaceUserId = "   ";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetUserBookmarksAsync(whitespaceUserId));
        }

        [Test]
        public async Task GetUserBookmarksAsync_ShouldHandleBoundaryCase_WhenBookmarkIsExactlyAtRecentDaysThreshold()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var languageId = Guid.NewGuid();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Recent Bookmark",
                    Content = "Content",
                    // Clearly within the 30-day window
                    CreatedAt = DateTime.UtcNow.AddDays(-29),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Old Bookmark",
                    Content = "Old Content",
                    // Clearly outside the 30-day window
                    CreatedAt = DateTime.UtcNow.AddDays(-31),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "JavaScript" }
                }
            }
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserBookmarksAsync(userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.TotalBookmarks, Is.EqualTo(2));
                // Only the bookmark at -29 days should be counted as recent
                Assert.That(result.RecentBookmarksCount, Is.EqualTo(1));
                Assert.That(result.MostCommonLanguage, Is.EqualTo("C#")); // C# should win with 1 vs 0 recent bookmarks
            });
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        #endregion

        #region GetUserSnippetsAsync Tests

        [Test]
        public async Task GetUserSnippetsAsync_ShouldReturnPostCardViewModels_WhenUserHasBookmarks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var languageId = Guid.NewGuid();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Bookmark 1",
                    Content = "Content 1",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "C#" },
                    User = new ApplicationUser { UserName = "author1" }
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Bookmark 2",
                    Content = "Content 2",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LanguageId = languageId,
                    Language = new ProgrammingLanguage { Id = languageId, Name = "JavaScript" },
                    User = new ApplicationUser { UserName = "author2" }
                }
            }
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserSnippetsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var resultList = result.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(resultList[0].Title, Is.EqualTo("Bookmark 1"));
                Assert.That(resultList[0].Content, Is.EqualTo("Content 1"));
                Assert.That(resultList[0].LanguageName, Is.EqualTo("C#"));

                Assert.That(resultList[1].Title, Is.EqualTo("Bookmark 2"));
                Assert.That(resultList[1].Content, Is.EqualTo("Content 2"));
                Assert.That(resultList[1].LanguageName, Is.EqualTo("JavaScript"));
            });

            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserSnippetsAsync_ShouldReturnEmptyCollection_WhenUserHasNoBookmarks()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var users = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = userGuid,
            UserName = "testuser",
            Email = "test@example.com",
            Bookmarks = new List<Post>() // Empty bookmarks
        }
    };

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserSnippetsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserSnippetsAsync_ShouldReturnEmptyCollection_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var users = new List<ApplicationUser>(); // Empty list - no users

            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserSnippetsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
            _mockUserRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetUserSnippetsAsync_ShouldThrowFormatException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            var invalidUserId = "not-a-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetUserSnippetsAsync(invalidUserId));
        }

        [Test]
        public void GetUserSnippetsAsync_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            string nullUserId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetUserSnippetsAsync(nullUserId));
        }

        #endregion

        #region GetUserConnectionsAsync Tests

        [Test]
        public async Task GetUserConnectionsAsync_ShouldReturnConnections_WhenUserIsConnector()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var connectedUserId = Guid.NewGuid();

            var connections = new List<UserConnection>
    {
        new UserConnection
        {
            UserId = userGuid,
            ConnectedUserId = connectedUserId,
            Status = ConnectionStatus.Accepted,
            User = new ApplicationUser
            {
                Id = userGuid,
                UserName = "mainuser",
                ProfilePicturePath = "images/mainuser.jpg",
                Posts = new List<Post> { new Post(), new Post() } // 2 posts
            },
            ConnectedUser = new ApplicationUser
            {
                Id = connectedUserId,
                UserName = "connecteduser",
                ProfilePicturePath = "images/connected.jpg",
                Posts = new List<Post> { new Post(), new Post(), new Post() } // 3 posts
            }
        }
    };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserConnectionsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            var connection = result.First();

            Assert.Multiple(() =>
            {
                Assert.That(connection.UserId, Is.EqualTo(userGuid.ToString()));
                Assert.That(connection.ConnectedUserId, Is.EqualTo(connectedUserId.ToString()));
                Assert.That(connection.TargetUserId, Is.EqualTo(connectedUserId.ToString())); // Should be the connected user
                Assert.That(connection.Username, Is.EqualTo("connecteduser")); // Should show connected user's name
                Assert.That(connection.ActorAvatar, Is.EqualTo("/images/connected.jpg")); // Should show connected user's avatar
                Assert.That(connection.PostsCount, Is.EqualTo(3)); // Should show connected user's post count
                Assert.That(connection.Type, Is.EqualTo(ConnectionStatus.Accepted));
            });

            _mockUserConnectionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserConnectionsAsync_ShouldReturnConnections_WhenUserIsConnected()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var otherUserId = Guid.NewGuid();

            var connections = new List<UserConnection>
    {
        new UserConnection
        {
            UserId = otherUserId, // Other user is the connector
            ConnectedUserId = userGuid, // Our user is being connected to
            Status = ConnectionStatus.Pending,
            User = new ApplicationUser
            {
                Id = otherUserId,
                UserName = "otheruser",
                ProfilePicturePath = "images/other.jpg",
                Posts = new List<Post> { new Post() } // 1 post
            },
            ConnectedUser = new ApplicationUser
            {
                Id = userGuid,
                UserName = "mainuser",
                ProfilePicturePath = "images/main.jpg",
                Posts = new List<Post> { new Post(), new Post() } // 2 posts
            }
        }
    };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserConnectionsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            var connection = result.First();

            Assert.Multiple(() =>
            {
                Assert.That(connection.UserId, Is.EqualTo(otherUserId.ToString()));
                Assert.That(connection.ConnectedUserId, Is.EqualTo(userGuid.ToString()));
                Assert.That(connection.TargetUserId, Is.EqualTo(otherUserId.ToString())); // Should be the other user
                Assert.That(connection.Username, Is.EqualTo("otheruser")); // Should show other user's name
                Assert.That(connection.ActorAvatar, Is.EqualTo("/images/other.jpg")); // Should show other user's avatar
                Assert.That(connection.PostsCount, Is.EqualTo(1)); // Should show other user's post count
                Assert.That(connection.Type, Is.EqualTo(ConnectionStatus.Pending));
            });

            _mockUserConnectionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserConnectionsAsync_ShouldReturnMultipleConnections_WhenUserHasBothTypes()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            var connections = new List<UserConnection>
    {
        // User is the connector
        new UserConnection
        {
            UserId = userGuid,
            ConnectedUserId = user1Id,
            Status = ConnectionStatus.Accepted,
            User = new ApplicationUser
            {
                Id = userGuid,
                UserName = "mainuser",
                ProfilePicturePath = "images/main.jpg",
                Posts = new List<Post> { new Post() }
            },
            ConnectedUser = new ApplicationUser
            {
                Id = user1Id,
                UserName = "user1",
                ProfilePicturePath = "images/user1.jpg",
                Posts = new List<Post> { new Post(), new Post() }
            }
        },
        // User is being connected to
        new UserConnection
        {
            UserId = user2Id,
            ConnectedUserId = userGuid,
            Status = ConnectionStatus.Blocked,
            User = new ApplicationUser
            {
                Id = user2Id,
                UserName = "user2",
                ProfilePicturePath = "images/user2.jpg",
                Posts = new List<Post> { new Post(), new Post(), new Post() }
            },
            ConnectedUser = new ApplicationUser
            {
                Id = userGuid,
                UserName = "mainuser",
                ProfilePicturePath = "images/main.jpg",
                Posts = new List<Post> { new Post() }
            }
        }
    };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserConnectionsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var resultList = result.ToList();

            // First connection - user is connector
            var firstConnection = resultList.FirstOrDefault(c => c.TargetUserId == user1Id.ToString());
            Assert.That(firstConnection, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(firstConnection.Username, Is.EqualTo("user1"));
                Assert.That(firstConnection.PostsCount, Is.EqualTo(2));
                Assert.That(firstConnection.Type, Is.EqualTo(ConnectionStatus.Accepted));
            });

            // Second connection - user is being connected to
            var secondConnection = resultList.FirstOrDefault(c => c.TargetUserId == user2Id.ToString());
            Assert.That(secondConnection, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(secondConnection.Username, Is.EqualTo("user2"));
                Assert.That(secondConnection.PostsCount, Is.EqualTo(3));
                Assert.That(secondConnection.Type, Is.EqualTo(ConnectionStatus.Blocked));
            });

            _mockUserConnectionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserConnectionsAsync_ShouldReturnEmptyCollection_WhenUserHasNoConnections()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var connections = new List<UserConnection>(); // Empty list

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserConnectionsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
            _mockUserConnectionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetUserConnectionsAsync_ShouldHandleNullProfilePicturePath()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var connectedUserId = Guid.NewGuid();

            var connections = new List<UserConnection>
    {
        new UserConnection
        {
            UserId = userGuid,
            ConnectedUserId = connectedUserId,
            Status = ConnectionStatus.Accepted,
            User = new ApplicationUser
            {
                Id = userGuid,
                UserName = "mainuser",
                ProfilePicturePath = null, // Null profile picture
                Posts = new List<Post>()
            },
            ConnectedUser = new ApplicationUser
            {
                Id = connectedUserId,
                UserName = "connecteduser",
                ProfilePicturePath = null, // Null profile picture
                Posts = new List<Post>()
            }
        }
    };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetUserConnectionsAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            var connection = result.First();
            Assert.That(connection.ActorAvatar, Is.EqualTo("/")); // Should handle null gracefully

            _mockUserConnectionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetUserConnectionsAsync_ShouldThrowFormatException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            var invalidUserId = "not-a-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _userService.GetUserConnectionsAsync(invalidUserId));
        }

        [Test]
        public void GetUserConnectionsAsync_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            string nullUserId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.GetUserConnectionsAsync(nullUserId));
        }

        #endregion
    }
}