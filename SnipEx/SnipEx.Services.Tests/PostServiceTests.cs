namespace SnipEx.Services.Tests
{
    using System.Globalization;

    using Moq;
    using MockQueryable;
    using NUnit.Framework;
    using Microsoft.EntityFrameworkCore.Storage;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Services.Data.Models;
    using SnipEx.Web.ViewModels.Comment;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    using static SnipEx.Common.EntityValidationConstants.Post;

    [TestFixture]
    public class PostServiceTests
    {
        private Mock<IRepository<Tag, Guid>> _mockTagRepository;
        private Mock<IRepository<Post, Guid>> _mockPostRepository;
        private Mock<ITagService> _mockTagService;
        private Mock<ICommentService> _mockCommentService;
        private Mock<IUserActionService> _mockUserActionService;
        private PostService _postService;

        [SetUp]
        public void Setup()
        {
            _mockTagRepository = new Mock<IRepository<Tag, Guid>>();
            _mockPostRepository = new Mock<IRepository<Post, Guid>>();
            _mockTagService = new Mock<ITagService>();
            _mockCommentService = new Mock<ICommentService>();
            _mockUserActionService = new Mock<IUserActionService>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _postService = new PostService(
                _mockTagRepository.Object,
                _mockPostRepository.Object,
                _mockTagService.Object,
                _mockCommentService.Object,
                _mockUserActionService.Object);
        }

        #region GetPostsAsync Tests

        [Test]
        public async Task GetPostsAsync_ShouldReturnAllPosts_WhenNoFiltersApplied()
        {
            // Arrange
            var posts = CreateMockPosts();
            var tags = CreateMockTags();

            var mockPostsQueryable = posts.AsQueryable().BuildMock();
            var mockTagsQueryable = tags.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockTagRepository.Setup(r => r.GetAllAttached()).Returns(mockTagsQueryable);

            // Act
            var result = await _postService.GetPostsAsync(null, null, null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Posts.Count, Is.EqualTo(2));
            Assert.That(result.PopularTags.Count, Is.EqualTo(2));
            Assert.That(result.SearchQuery, Is.Null);
            Assert.That(result.SelectedTag, Is.Null);
        }

        [Test]
        public async Task GetPostsAsync_ShouldFilterByTag_WhenTagProvided()
        {
            // Arrange
            var posts = CreateMockPosts();
            var tags = CreateMockTags();

            var mockPostsQueryable = posts.AsQueryable().BuildMock();
            var mockTagsQueryable = tags.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockTagRepository.Setup(r => r.GetAllAttached()).Returns(mockTagsQueryable);

            // Act
            var result = await _postService.GetPostsAsync("CSharp", null, null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SelectedTag, Is.EqualTo("CSharp"));
            Assert.That(result.Posts.Any(p => p.Tags.Any(t => t.Name == "CSharp")), Is.True);
        }

        [Test]
        public async Task GetPostsAsync_ShouldFilterBySearch_WhenSearchProvided()
        {
            // Arrange
            var posts = CreateMockPosts();
            var tags = CreateMockTags();

            var mockPostsQueryable = posts.AsQueryable().BuildMock();
            var mockTagsQueryable = tags.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockTagRepository.Setup(r => r.GetAllAttached()).Returns(mockTagsQueryable);

            // Act
            var result = await _postService.GetPostsAsync(null, "test", null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchQuery, Is.EqualTo("test"));
        }

        [Test]
        public async Task GetPostsAsync_ShouldSortByPopular_WhenSortIsPopular()
        {
            // Arrange
            var posts = CreateMockPosts();
            var tags = CreateMockTags();

            var mockPostsQueryable = posts.AsQueryable().BuildMock();
            var mockTagsQueryable = tags.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockTagRepository.Setup(r => r.GetAllAttached()).Returns(mockTagsQueryable);

            // Act
            var result = await _postService.GetPostsAsync(null, null, "popular");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CurrentSort, Is.EqualTo("popular"));
        }

        [Test]
        public async Task GetPostsAsync_ShouldSortByLikes_WhenSortIsLikes()
        {
            // Arrange
            var posts = CreateMockPosts();
            var tags = CreateMockTags();

            var mockPostsQueryable = posts.AsQueryable().BuildMock();
            var mockTagsQueryable = tags.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockTagRepository.Setup(r => r.GetAllAttached()).Returns(mockTagsQueryable);

            // Act
            var result = await _postService.GetPostsAsync(null, null, "likes");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CurrentSort, Is.EqualTo("likes"));
        }

        #endregion

        #region GetPostsCardsAsync Tests

        [Test]
        public async Task GetPostsCardsAsync_ShouldReturnPostCards_WhenCalled()
        {
            // Arrange
            var posts = CreateMockPostsWithLanguage();
            var mockPostsQueryable = posts.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);

            // Act
            var result = await _postService.GetPostsCardsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        #endregion

        #region GetPostsCardsByIdAsync Tests

        [Test]
        public async Task GetPostsCardsByIdAsync_ShouldReturnUserPosts_WhenValidUserId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var posts = CreateMockPostsWithLanguage().Where(p => p.UserId == userGuid).ToList();
            var mockPostsQueryable = posts.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);

            // Act
            var result = await _postService.GetPostsCardsByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetPostsCardsByIdAsync_ShouldThrowException_WhenInvalidUserId()
        {
            // Arrange
            var invalidUserId = "invalid-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _postService.GetPostsCardsByIdAsync(invalidUserId));
        }

        #endregion

        #region AddPostAsync Tests

        [Test]
        public async Task AddPostAsync_ShouldReturnTrue_WhenValidInput()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture),
                LanguageId = Guid.NewGuid().ToString(),
                Tags = new List<AddTagFormModel>
                {
                    new AddTagFormModel { Name = "CSharp" },
                    new AddTagFormModel { Name = "Testing" }
                }
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockPostRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockPostRepository.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
            _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockTagService.Setup(s => s.AddTagsToPostAsync(It.IsAny<IEnumerable<AddTagFormModel>>(), It.IsAny<Guid>()))
                          .ReturnsAsync(true);

            // Act
            var result = await _postService.AddPostAsync(model, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockPostRepository.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Once);
            _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockTagService.Verify(s => s.AddTagsToPostAsync(It.IsAny<IEnumerable<AddTagFormModel>>(), It.IsAny<Guid>()), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }

        [Test]
        public async Task AddPostAsync_ShouldReturnFalse_WhenInvalidCreatedAtFormat()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = "invalid-date-format",
                LanguageId = Guid.NewGuid().ToString()
            };

            // Act
            var result = await _postService.AddPostAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockPostRepository.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Never);
        }

        [Test]
        public async Task AddPostAsync_ShouldReturnFalse_WhenTagServiceFails()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture),
                LanguageId = Guid.NewGuid().ToString(),
                Tags = new List<AddTagFormModel> { new AddTagFormModel { Name = "CSharp" } }
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockPostRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockPostRepository.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
            _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockTagService.Setup(s => s.AddTagsToPostAsync(It.IsAny<IEnumerable<AddTagFormModel>>(), It.IsAny<Guid>()))
                          .ReturnsAsync(false);

            // Act
            var result = await _postService.AddPostAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            mockTransaction.Verify(t => t.RollbackAsync(default), Times.Once);
        }

        [Test]
        public async Task AddPostAsync_ShouldReturnFalse_WhenExceptionThrown()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture),
                LanguageId = Guid.NewGuid().ToString()
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockPostRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockPostRepository.Setup(r => r.AddAsync(It.IsAny<Post>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _postService.AddPostAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            mockTransaction.Verify(t => t.RollbackAsync(default), Times.Once);
        }

        [Test]
        public async Task AddPostAsync_ShouldReturnTrue_WhenNoTags()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture),
                LanguageId = Guid.NewGuid().ToString()
            };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockPostRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockPostRepository.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
            _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _postService.AddPostAsync(model, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockTagService.Verify(s => s.AddTagsToPostAsync(It.IsAny<IEnumerable<AddTagFormModel>>(), It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region GetPostByIdAsync Tests

        [Test]
        public async Task GetPostByIdAsync_ShouldReturnPost_WhenValidIdAndNoUser()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = CreateMockPost(postId);
            var posts = new List<Post> { post };
            var mockPostsQueryable = posts.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockCommentService.Setup(s => s.GetStructuredComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                              .ReturnsAsync(new List<CommentViewModel>());

            // Act
            var result = await _postService.GetPostByIdAsync(postId, null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(postId.ToString()));
            Assert.That(result.IsLikedByCurrentUser, Is.False);
            Assert.That(result.IsBookmarkedByCurrentUser, Is.False);
        }

        [Test]
        public async Task GetPostByIdAsync_ShouldReturnPostWithUserActions_WhenValidIdAndUser()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var post = CreateMockPost(postId);
            var posts = new List<Post> { post };
            var mockPostsQueryable = posts.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);
            _mockCommentService.Setup(s => s.GetStructuredComments(It.IsAny<IEnumerable<CommentViewModel>>()))
                              .ReturnsAsync(new List<CommentViewModel>());
            _mockUserActionService.Setup(s => s.IsPostLikedByUserAsync(postId, userId)).ReturnsAsync(true);
            _mockUserActionService.Setup(s => s.IsPostSavedByUserAsync(postId, userId)).ReturnsAsync(true);

            // Act
            var result = await _postService.GetPostByIdAsync(postId, userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsLikedByCurrentUser, Is.True);
            Assert.That(result.IsBookmarkedByCurrentUser, Is.True);
            _mockCommentService.Verify(s => s.SetUserLikeStatus(It.IsAny<CommentViewModel>(), It.IsAny<ICollection<Comment>>(), userId), Times.Never);
        }

        [Test]
        public async Task GetPostByIdAsync_ShouldReturnNull_WhenPostNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var posts = new List<Post>();
            var mockPostsQueryable = posts.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostsQueryable);

            // Act
            var result = await _postService.GetPostByIdAsync(postId, null);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Helper Methods

        private List<Post> CreateMockPosts()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
            var tag1 = new Tag { Id = Guid.NewGuid(), Name = "CSharp" };
            var tag2 = new Tag { Id = Guid.NewGuid(), Name = "Testing" };

            return new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Post 1",
                    Content = "Test content 1",
                    Views = 100,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    User = user,
                    UserId = user.Id,
                    PostsTags = new List<PostTag>
                    {
                        new PostTag { Tag = tag1, TagId = tag1.Id }
                    },
                    Comments = new List<Comment>(),
                    Likes = new List<PostLike>()
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Post 2",
                    Content = "Test content 2",
                    Views = 50,
                    CreatedAt = DateTime.UtcNow,
                    User = user,
                    UserId = user.Id,
                    PostsTags = new List<PostTag>
                    {
                        new PostTag { Tag = tag2, TagId = tag2.Id }
                    },
                    Comments = new List<Comment>(),
                    Likes = new List<PostLike> { new PostLike() }
                }
            };
        }

        private List<Tag> CreateMockTags()
        {
            return new List<Tag>
            {
                new Tag { Id = Guid.NewGuid(), Name = "CSharp", PostsTags = new List<PostTag> { new PostTag() } },
                new Tag { Id = Guid.NewGuid(), Name = "Testing", PostsTags = new List<PostTag>() }
            };
        }

        private List<Post> CreateMockPostsWithLanguage()
        {
            var userId = Guid.NewGuid();
            var language = new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "C#" };

            return new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Post 1",
                    Content = "Content 1",
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    Language = language,
                    LanguageId = language.Id
                },
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Post 2",
                    Content = "Content 2",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UserId = Guid.NewGuid(),
                    Language = language,
                    LanguageId = language.Id
                }
            };
        }

        private Post CreateMockPost(Guid postId)
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
            var language = new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "C#" };
            var tag = new Tag { Id = Guid.NewGuid(), Name = "CSharp" };

            return new Post
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test Content",
                Views = 10,
                CreatedAt = DateTime.UtcNow,
                User = user,
                UserId = user.Id,
                Language = language,
                LanguageId = language.Id,
                PostsTags = new List<PostTag>
                {
                    new PostTag { Tag = tag, TagId = tag.Id }
                },
                Comments = new List<Comment>(),
                Likes = new List<PostLike>()
            };
        }

        #endregion
    }
}