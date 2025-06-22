namespace SnipEx.Services.Tests
{
    using System.Globalization;

    using Moq;
    using MediatR;
    using MockQueryable;
    using NUnit.Framework;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Services.Data.Models;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Web.ViewModels.Comment;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Comments.ReplyAdded;
    using SnipEx.Services.Mediator.Comments.CommentAdded;

    using static SnipEx.Common.EntityValidationConstants.Comment;
    
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<IRepository<Comment, Guid>> _mockCommentRepository;
        private Mock<IRepository<Post, Guid>> _mockPostRepository;
        private Mock<IMediator> _mockMediator;
        private CommentService _commentService;

        [SetUp]
        public void Setup()
        {
            _mockCommentRepository = new Mock<IRepository<Comment, Guid>>();
            _mockPostRepository = new Mock<IRepository<Post, Guid>>();
            _mockMediator = new Mock<IMediator>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _commentService = new CommentService(
                _mockCommentRepository.Object,
                _mockPostRepository.Object,
                _mockMediator.Object);
        }

        #region AddCommentAsync Tests

        [Test]
        public async Task AddCommentAsync_ShouldReturnTrue_WhenValidInput()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostCommentFormModel
            {
                Content = "Test comment",
                PostId = postGuid.ToString(),
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            _mockCommentRepository.Setup(r => r.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            _mockCommentRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMediator.Setup(m => m.Publish(It.IsAny<CommentAddedEvent>(), default)).Returns(Task.CompletedTask);

            // Act
            var result = await _commentService.AddCommentAsync(model, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            _mockCommentRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<CommentAddedEvent>(), default), Times.Once);
        }

        [Test]
        public async Task AddCommentAsync_ShouldReturnFalse_WhenInvalidPostId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostCommentFormModel
            {
                Content = "Test comment",
                PostId = "invalid-guid",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            // Act
            var result = await _commentService.AddCommentAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => 
                r.AddAsync(It.IsAny<Comment>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<CommentAddedEvent>(), default), Times.Never);
        }

        [Test]
        public async Task AddCommentAsync_ShouldReturnFalse_WhenInvalidCreatedAtFormat()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var model = new AddPostCommentFormModel
            {
                Content = "Test comment",
                PostId = postGuid.ToString(),
                CreatedAt = "invalid-date-format"
            };

            // Act
            var result = await _commentService.AddCommentAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<CommentAddedEvent>(), default), Times.Never);
        }

        #endregion

        #region AddReplyAsync Tests

        [Test]
        public async Task AddReplyAsync_ShouldReturnTrue_WhenValidInput()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var parentCommentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var parentComment = new Comment { Id = parentCommentGuid, ParentCommentId = null };

            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = postGuid.ToString(),
                ParentCommentId = parentCommentGuid.ToString(),
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            _mockCommentRepository.Setup(r => r.GetByIdAsync(parentCommentGuid)).ReturnsAsync(parentComment);
            _mockCommentRepository.Setup(r => r.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            _mockCommentRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMediator.Setup(m => m.Publish(It.IsAny<ReplyAddedEvent>(), default)).Returns(Task.CompletedTask);

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            _mockCommentRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.Is<ReplyAddedEvent>(e => e.IsMention == false), default), Times.Once);
        }

        [Test]
        public async Task AddReplyAsync_ShouldSetIsMentionTrue_WhenReferenceCommentIdIsProvided()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var parentCommentGuid = Guid.NewGuid();
            var referenceCommentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var parentComment = new Comment
            {
                Id = parentCommentGuid,
                ParentCommentId = null
            };

            var referenceComment = new Comment
            {
                Id = referenceCommentGuid
            };

            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = postGuid.ToString(),
                ParentCommentId = parentCommentGuid.ToString(),
                ReferenceCommentId = referenceCommentGuid.ToString(),
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            _mockCommentRepository.Setup(r =>
                r.GetByIdAsync(parentCommentGuid))
                .ReturnsAsync(parentComment);
            _mockCommentRepository.Setup(r =>
                r.GetByIdAsync(referenceCommentGuid))
                .ReturnsAsync(referenceComment);
            _mockCommentRepository.Setup(r =>
                r.AddAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);
            _mockCommentRepository.Setup(r =>
                r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockMediator.Verify(m => m.Publish(It.Is<ReplyAddedEvent>(e => e.IsMention == true), default), Times.Once);
        }

        [Test]
        public async Task AddReplyAsync_ShouldReturnFalse_WhenInvalidPostId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = "invalid-guid",
                ParentCommentId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task AddReplyAsync_ShouldReturnFalse_WhenInvalidParentCommentId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = Guid.NewGuid().ToString(),
                ParentCommentId = "invalid-guid",
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task AddReplyAsync_ShouldReturnFalse_WhenParentCommentNotFound()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var parentCommentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = postGuid.ToString(),
                ParentCommentId = parentCommentGuid.ToString(),
                CreatedAt = DateTime.UtcNow.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)
            };

            _mockCommentRepository.Setup(r => r.GetByIdAsync(parentCommentGuid)).ReturnsAsync((Comment)null);

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task AddReplyAsync_ShouldReturnFalse_WhenInvalidCreatedAtFormat()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var parentCommentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var model = new AddCommentReplyFormModel
            {
                Content = "Test reply",
                PostId = postGuid.ToString(),
                ParentCommentId = parentCommentGuid.ToString(),
                CreatedAt = "invalid-date-format"
            };

            // Act
            var result = await _commentService.AddReplyAsync(model, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        #endregion

        #region GetStructuredComments Tests

        [Test]
        public async Task GetStructuredComments_ShouldReturnOnlyTopLevelComments()
        {
            // Arrange
            var comments = new List<CommentViewModel>
            {
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = null },
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = Guid.NewGuid().ToString() },
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = null },
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = Guid.NewGuid().ToString() }
            };

            // Act
            var result = await _commentService.GetStructuredComments(comments);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => c.ParentCommentId == null), Is.True);
        }

        [Test]
        public async Task GetStructuredComments_ShouldReturnEmptyList_WhenNoTopLevelComments()
        {
            // Arrange
            var comments = new List<CommentViewModel>
            {
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = Guid.NewGuid().ToString() },
                new CommentViewModel { Id = Guid.NewGuid().ToString(), ParentCommentId = Guid.NewGuid().ToString() }
            };

            // Act
            var result = await _commentService.GetStructuredComments(comments);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        #endregion

        #region GetCommentsByPostIdAsync Tests

        [Test]
        public async Task GetCommentsByPostIdAsync_ShouldReturnComments_WhenCommentsExist()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    PostId = postGuid,
                    Content = "Comment 1",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    User = new ApplicationUser { UserName = "User1" }
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    PostId = postGuid,
                    Content = "Comment 2",
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    User = new ApplicationUser { UserName = "User2" }
                }
            };

            var mockQueryable = comments.AsQueryable().BuildMock();
            _mockCommentRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _commentService.GetCommentsByPostIdAsync(postGuid);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCommentsByPostIdAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var comments = new List<Comment>();

            var mockQueryable = comments.AsQueryable().BuildMock();
            _mockCommentRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _commentService.GetCommentsByPostIdAsync(postGuid);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        #endregion

        #region SetUserLikeStatus Tests

        [Test]
        public void SetUserLikeStatus_ShouldSetLikeStatus_WhenUserLikedComment()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var commentGuid = Guid.NewGuid();

            var comment = new CommentViewModel
            {
                Id = commentGuid.ToString(),
                Replies = new List<CommentViewModel>()
            };

            var postComments = new List<Comment>
            {
                new Comment
                {
                    Id = commentGuid,
                    Likes = new List<CommentLike>
                    {
                        new CommentLike { UserId = userGuid }
                    }
                }
            };

            // Act
            _commentService.SetUserLikeStatus(comment, postComments, userId);

            // Assert
            Assert.That(comment.IsLikedByCurrentUser, Is.True);
        }

        [Test]
        public void SetUserLikeStatus_ShouldNotSetLikeStatus_WhenUserDidNotLikeComment()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var commentGuid = Guid.NewGuid();

            var comment = new CommentViewModel
            {
                Id = commentGuid.ToString(),
                Replies = new List<CommentViewModel>()
            };

            var postComments = new List<Comment>
            {
                new Comment
                {
                    Id = commentGuid,
                    Likes = new List<CommentLike>()
                }
            };

            // Act
            _commentService.SetUserLikeStatus(comment, postComments, userId);

            // Assert
            Assert.That(comment.IsLikedByCurrentUser, Is.False);
        }

        [Test]
        public void SetUserLikeStatus_ShouldHandleInvalidCommentId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var comment = new CommentViewModel
            {
                Id = "invalid-guid",
                Replies = new List<CommentViewModel>()
            };
            var postComments = new List<Comment>();

            // Act & Assert (Should not throw)
            Assert.DoesNotThrow(() => _commentService.SetUserLikeStatus(comment, postComments, userId));
        }

        [Test]
        public void SetUserLikeStatus_ShouldProcessRepliesRecursively()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var parentCommentGuid = Guid.NewGuid();
            var replyCommentGuid = Guid.NewGuid();

            var reply = new CommentViewModel
            {
                Id = replyCommentGuid.ToString(),
                Replies = new List<CommentViewModel>()
            };

            var comment = new CommentViewModel
            {
                Id = parentCommentGuid.ToString(),
                Replies = new List<CommentViewModel> { reply }
            };

            var postComments = new List<Comment>
            {
                new Comment
                {
                    Id = parentCommentGuid,
                    Likes = new List<CommentLike>()
                },
                new Comment
                {
                    Id = replyCommentGuid,
                    Likes = new List<CommentLike>
                    {
                        new CommentLike { UserId = userGuid }
                    }
                }
            };

            // Act
            _commentService.SetUserLikeStatus(comment, postComments, userId);

            // Assert
            Assert.That(comment.IsLikedByCurrentUser, Is.False);
            Assert.That(reply.IsLikedByCurrentUser, Is.True);
        }

        #endregion

        #region GetReceivedCommentsCountAsync Tests

        [Test]
        public async Task GetReceivedCommentsCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var otherUserGuid = Guid.NewGuid();
            var userPostId = Guid.NewGuid();

            var posts = new List<Post>
            {
                new Post { Id = userPostId, UserId = userGuid }
            };

            var comments = new List<Comment>
            {
                new Comment { PostId = userPostId, UserId = otherUserGuid }, // Should count
                new Comment { PostId = userPostId, UserId = userGuid },     // Should not count (own comment)
                new Comment { PostId = userPostId, UserId = null },         // Should not count (no user)
                new Comment { PostId = Guid.NewGuid(), UserId = otherUserGuid } // Should not count (different post)
            };

            var mockPostQueryable = posts.AsQueryable().BuildMock();
            var mockCommentQueryable = comments.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostQueryable);
            _mockCommentRepository.Setup(r => r.GetAllAttached()).Returns(mockCommentQueryable);

            // Act
            var result = await _commentService.GetReceivedCommentsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GetReceivedCommentsCountAsync_ShouldReturnZero_WhenNoReceivedComments()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid(), UserId = userGuid }
            };

            var comments = new List<Comment>();

            var mockPostQueryable = posts.AsQueryable().BuildMock();
            var mockCommentQueryable = comments.AsQueryable().BuildMock();

            _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockPostQueryable);
            _mockCommentRepository.Setup(r => r.GetAllAttached()).Returns(mockCommentQueryable);

            // Act
            var result = await _commentService.GetReceivedCommentsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion
    }
}