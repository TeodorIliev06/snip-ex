namespace SnipEx.Services.Tests
{
    using Moq;
    using MediatR;
    using MockQueryable.Moq;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Posts.PostLiked;
    using SnipEx.Services.Mediator.Comments.CommentLiked;
    using SnipEx.Services.Mediator.Profiles.UserConnection;

    [TestFixture]
    public class UserActionServiceTests
    {
        private Mock<IRepository<Post, Guid>> _mockPostRepository;
        private Mock<IRepository<ApplicationUser, Guid>> _mockUserRepository;
        private Mock<IRepository<PostLike, Guid>> _mockPostLikeRepository;
        private Mock<IRepository<CommentLike, Guid>> _mockCommentLikeRepository;
        private Mock<IRepository<UserConnection, object>> _mockUserConnectionRepository;
        private Mock<IMediator> _mockMediator;
        private UserActionService _userActionService;

        [SetUp]
        public void Setup()
        {
            _mockPostRepository = new Mock<IRepository<Post, Guid>>();
            _mockUserRepository = new Mock<IRepository<ApplicationUser, Guid>>();
            _mockPostLikeRepository = new Mock<IRepository<PostLike, Guid>>();
            _mockCommentLikeRepository = new Mock<IRepository<CommentLike, Guid>>();
            _mockUserConnectionRepository = new Mock<IRepository<UserConnection, object>>();
            _mockMediator = new Mock<IMediator>();

            _userActionService = new UserActionService(
                _mockPostRepository.Object,
                _mockUserRepository.Object,
                _mockPostLikeRepository.Object,
                _mockCommentLikeRepository.Object,
                _mockUserConnectionRepository.Object,
                _mockMediator.Object);
        }

        #region TogglePostLikeAsync Tests

        [Test]
        public async Task TogglePostLikeAsync_ShouldRemoveLike_WhenLikeExists()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var existingLike = new PostLike
            {
                Id = Guid.NewGuid(),
                PostId = postGuid,
                UserId = userGuid
            };

            _mockPostLikeRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<PostLike, bool>>>()))
                .ReturnsAsync(existingLike);

            // Act
            var result = await _userActionService.TogglePostLikeAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockPostLikeRepository.Verify(r => r.DeleteAsync(existingLike.Id), Times.Once);
            _mockPostLikeRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<PostLikedEvent>(), default), Times.Never);
        }

        [Test]
        public async Task TogglePostLikeAsync_ShouldAddLike_WhenLikeDoesNotExist()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            _mockPostLikeRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<PostLike, bool>>>()))
                .ReturnsAsync((PostLike)null);

            // Act
            var result = await _userActionService.TogglePostLikeAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockPostLikeRepository.Verify(r => r.AddAsync(It.IsAny<PostLike>()), Times.Once);
            _mockPostLikeRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(
                m => m.Publish(It.Is<PostLikedEvent>(e => e.PostGuid == postGuid && e.ActorGuid == userGuid), default),
                Times.Once);
        }

        #endregion

        #region IsPostLikedByUserAsync Tests

        [Test]
        public async Task IsPostLikedByUserAsync_ShouldReturnTrue_WhenPostIsLikedByUser()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var postLikes = new List<PostLike>
            {
                new PostLike { PostId = postGuid, UserId = userGuid }
            };

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.IsPostLikedByUserAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsPostLikedByUserAsync_ShouldReturnFalse_WhenPostIsNotLikedByUser()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var postLikes = new List<PostLike>();
            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.IsPostLikedByUserAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetPostLikesCountAsync Tests

        [Test]
        public async Task GetPostLikesCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var postLikes = new List<PostLike>
            {
                new PostLike { PostId = postGuid, UserId = Guid.NewGuid() },
                new PostLike { PostId = postGuid, UserId = Guid.NewGuid() },
                new PostLike { PostId = Guid.NewGuid(), UserId = Guid.NewGuid() } // Different post
            };

            var mockDbSet = postLikes.AsQueryable().BuildMockDbSet();
            _mockPostLikeRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.GetPostLikesCountAsync(postGuid);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion

        #region ToggleCommentLikeAsync Tests

        [Test]
        public async Task ToggleCommentLikeAsync_ShouldRemoveLike_WhenLikeExists()
        {
            // Arrange
            var commentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var existingLike = new CommentLike
            {
                Id = Guid.NewGuid(),
                CommentId = commentGuid,
                UserId = userGuid
            };

            _mockCommentLikeRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<CommentLike, bool>>>()))
                .ReturnsAsync(existingLike);

            // Act
            var result = await _userActionService.ToggleCommentLikeAsync(commentGuid, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockCommentLikeRepository.Verify(r => r.DeleteAsync(existingLike.Id), Times.Once);
            _mockCommentLikeRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ToggleCommentLikeAsync_ShouldAddLike_WhenLikeDoesNotExist()
        {
            // Arrange
            var commentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            _mockCommentLikeRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<CommentLike, bool>>>()))
                .ReturnsAsync((CommentLike)null);

            // Act
            var result = await _userActionService.ToggleCommentLikeAsync(commentGuid, userId);

            // Assert
            Assert.That(result, Is.True);
            _mockCommentLikeRepository.Verify(r => r.AddAsync(It.IsAny<CommentLike>()), Times.Once);
            _mockCommentLikeRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(
                m => m.Publish(It.Is<CommentLikedEvent>(e => e.CommentGuid == commentGuid && e.ActorGuid == userGuid),
                    default), Times.Once);
        }

        #endregion

        #region IsCommentLikedByUserAsync Tests

        [Test]
        public async Task IsCommentLikedByUserAsync_ShouldReturnTrue_WhenCommentIsLikedByUser()
        {
            // Arrange
            var commentGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var commentLikes = new List<CommentLike>
            {
                new CommentLike { CommentId = commentGuid, UserId = userGuid }
            };

            var mockDbSet = commentLikes.AsQueryable().BuildMockDbSet();
            _mockCommentLikeRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.IsCommentLikedByUserAsync(commentGuid, userId);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region GetCommentLikesCountAsync Tests

        [Test]
        public async Task GetCommentLikesCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var commentGuid = Guid.NewGuid();
            var commentLikes = new List<CommentLike>
            {
                new CommentLike { CommentId = commentGuid, UserId = Guid.NewGuid() },
                new CommentLike { CommentId = commentGuid, UserId = Guid.NewGuid() },
                new CommentLike { CommentId = Guid.NewGuid(), UserId = Guid.NewGuid() } // Different comment
            };

            var mockDbSet = commentLikes.AsQueryable().BuildMockDbSet();
            _mockCommentLikeRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.GetCommentLikesCountAsync(commentGuid);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion

        #region TogglePostSaveAsync Tests

        [Test]
        public async Task TogglePostSaveAsync_ShouldRemoveBookmark_WhenBookmarkExists()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var post = new Post { Id = postGuid, Title = "Test Post" };
            var user = new ApplicationUser
            {
                Id = userGuid,
                Bookmarks = new HashSet<Post> { post }
            };

            var users = new List<ApplicationUser> { user };
            var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetAllAttached()).Returns(mockUserDbSet.Object);
            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync(post);

            // Act
            var result = await _userActionService.TogglePostSaveAsync(postGuid, userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.False);
                Assert.That(user.Bookmarks.Contains(post), Is.False);
            });
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task TogglePostSaveAsync_ShouldAddBookmark_WhenBookmarkDoesNotExist()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var post = new Post { Id = postGuid, Title = "Test Post" };
            var user = new ApplicationUser
            {
                Id = userGuid,
                Bookmarks = new HashSet<Post>()
            };

            var users = new List<ApplicationUser> { user };
            var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetAllAttached()).Returns(mockUserDbSet.Object);
            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync(post);

            // Act
            var result = await _userActionService.TogglePostSaveAsync(postGuid, userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True);
                Assert.That(user.Bookmarks.Contains(post), Is.True);
            });
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task TogglePostSaveAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var users = new List<ApplicationUser>();
            var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetAllAttached()).Returns(mockUserDbSet.Object);

            // Act
            var result = await _userActionService.TogglePostSaveAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task TogglePostSaveAsync_ShouldReturnFalse_WhenPostNotFound()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var user = new ApplicationUser { Id = userGuid };

            var users = new List<ApplicationUser> { user };
            var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetAllAttached()).Returns(mockUserDbSet.Object);
            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync((Post)null);

            // Act
            var result = await _userActionService.TogglePostSaveAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsPostSavedByUserAsync Tests

        [Test]
        public async Task IsPostSavedByUserAsync_ShouldReturnTrue_WhenPostIsSaved()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var post = new Post { Id = postGuid };
            var user = new ApplicationUser
            {
                Id = userGuid,
                Bookmarks = new HashSet<Post> { post }
            };

            var users = new List<ApplicationUser> { user };
            var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetAllAttached()).Returns(mockUserDbSet.Object);

            // Act
            var result = await _userActionService.IsPostSavedByUserAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region ToggleConnectionAsync Tests

        [Test]
        public async Task ToggleConnectionAsync_ShouldReturnFalse_WhenUsersAreSame()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            // Act
            var result = await _userActionService.ToggleConnectionAsync(userId, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ToggleConnectionAsync_ShouldRemoveConnection_WhenConnectionExists()
        {
            // Arrange
            var targetUserId = Guid.NewGuid().ToString();
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserGuid = Guid.Parse(targetUserId);
            var currentUserGuid = Guid.Parse(currentUserId);

            var (smallerId, largerId) = currentUserGuid.CompareTo(targetUserGuid) < 0
                ? (currentUserGuid, targetUserGuid)
                : (targetUserGuid, currentUserGuid);

            var existingConnection = new UserConnection
            {
                UserId = smallerId,
                ConnectedUserId = largerId
            };

            _mockUserConnectionRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<UserConnection, bool>>>()))
                .ReturnsAsync(existingConnection);

            // Act
            var result = await _userActionService.ToggleConnectionAsync(targetUserId, currentUserId);

            // Assert
            Assert.That(result, Is.False);
            _mockUserConnectionRepository.Verify(r => r.DeleteAsync(existingConnection), Times.Once);
            _mockUserConnectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ToggleConnectionAsync_ShouldAddConnection_WhenConnectionDoesNotExist()
        {
            // Arrange
            var targetUserId = Guid.NewGuid().ToString();
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserGuid = Guid.Parse(targetUserId);
            var currentUserGuid = Guid.Parse(currentUserId);

            _mockUserConnectionRepository
                .Setup(r => r.FirstOrDefaultAsync(It
                    .IsAny<System.Linq.Expressions.Expression<System.Func<UserConnection, bool>>>()))
                .ReturnsAsync((UserConnection)null);

            // Act
            var result = await _userActionService.ToggleConnectionAsync(targetUserId, currentUserId);

            // Assert
            Assert.That(result, Is.True);
            _mockUserConnectionRepository.Verify(r => r.AddAsync(It.IsAny<UserConnection>()), Times.Once);
            _mockUserConnectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.Is<UserConnectionEvent>(e =>
                e.ActorGuid == currentUserGuid && e.TargetUserGuid == targetUserGuid), default), Times.Once);
        }

        #endregion

        #region DoesConnectionExistAsync Tests

        [Test]
        public async Task DoesConnectionExistAsync_ShouldReturnTrue_WhenConnectionExists()
        {
            // Arrange
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserId = Guid.NewGuid().ToString();
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuid = Guid.Parse(targetUserId);

            var (smallerId, largerId) = currentUserGuid.CompareTo(targetUserGuid) < 0
                ? (currentUserGuid, targetUserGuid)
                : (targetUserGuid, currentUserGuid);

            var connections = new List<UserConnection>
            {
                new UserConnection { UserId = smallerId, ConnectedUserId = largerId }
            };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.DoesConnectionExistAsync(currentUserId, targetUserId);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region GetConnectionsCountAsync Tests

        [Test]
        public async Task GetConnectionsCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var targetUserId = Guid.NewGuid().ToString();
            var targetUserGuid = Guid.Parse(targetUserId);
            var otherUser1 = Guid.NewGuid();
            var otherUser2 = Guid.NewGuid();

            var connections = new List<UserConnection>
            {
                new UserConnection { UserId = targetUserGuid, ConnectedUserId = otherUser1 },
                new UserConnection { UserId = otherUser2, ConnectedUserId = targetUserGuid },
                new UserConnection { UserId = otherUser1, ConnectedUserId = otherUser2 } // Unrelated connection
            };

            var mockDbSet = connections.AsQueryable().BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r => r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService.GetConnectionsCountAsync(targetUserId);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion

        #region GetMutualConnectionsCountByUserAsync Tests

        [Test]
        public async Task GetMutualConnectionsCountByUserAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserId1 = Guid.NewGuid().ToString();
            var targetUserId2 = Guid.NewGuid().ToString();
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuid1 = Guid.Parse(targetUserId1);
            var targetUserGuid2 = Guid.Parse(targetUserId2);

            var mutualUser1 = Guid.NewGuid();
            var mutualUser2 = Guid.NewGuid();
            var mutualUser3 = Guid.NewGuid();
            var nonMutualUser = Guid.NewGuid();

            var connections = new List<UserConnection>
            {
                // Current user connections
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = mutualUser1 },
                new UserConnection { UserId = mutualUser2, ConnectedUserId = currentUserGuid },
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = mutualUser3 },
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = nonMutualUser },
                
                // Target user 1 connections (shares mutualUser1 and mutualUser2 with current user)
                new UserConnection { UserId = targetUserGuid1, ConnectedUserId = mutualUser1 },
                new UserConnection { UserId = mutualUser2, ConnectedUserId = targetUserGuid1 },
                
                // Target user 2 connections (shares mutualUser1 and mutualUser3 with current user)
                new UserConnection { UserId = targetUserGuid2, ConnectedUserId = mutualUser1 },
                new UserConnection { UserId = mutualUser3, ConnectedUserId = targetUserGuid2 }
            };

            var mockDbSet = connections
                .AsQueryable()
                .BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r =>
                r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var targetUserIds = new List<string> { targetUserId1, targetUserId2 };
            var result = await _userActionService.GetMutualConnectionsCountByUserAsync(currentUserId, targetUserIds);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[targetUserId1], Is.EqualTo(2)); // mutualUser1 and mutualUser2
            Assert.That(result[targetUserId2], Is.EqualTo(2)); // mutualUser1 and mutualUser3
        }

        [Test]
        public async Task GetMutualConnectionsCountByUserAsync_WithSingleTarget_ShouldReturnCorrectCount()
        {
            // Arrange
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserId = Guid.NewGuid().ToString();
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuid = Guid.Parse(targetUserId);
            var mutualUser1 = Guid.NewGuid();
            var mutualUser2 = Guid.NewGuid();
            var nonMutualUser = Guid.NewGuid();

            var connections = new List<UserConnection>
            {
                // Current user connections
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = mutualUser1 },
                new UserConnection { UserId = mutualUser2, ConnectedUserId = currentUserGuid },
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = nonMutualUser },
                
                // Target user connections
                new UserConnection { UserId = targetUserGuid, ConnectedUserId = mutualUser1 },
                new UserConnection { UserId = mutualUser2, ConnectedUserId = targetUserGuid }
            };

            var mockDbSet = connections
                .AsQueryable()
                .BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r =>
                r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var targetUserIds = new List<string> { targetUserId };
            var result = await _userActionService
                .GetMutualConnectionsCountByUserAsync(currentUserId, targetUserIds);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[targetUserId], Is.EqualTo(2));
        }

        [Test]
        public async Task GetMutualConnectionsCountByUserAsync_WithNoMutualConnections_ShouldReturnZero()
        {
            // Arrange
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserId = Guid.NewGuid().ToString();
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuid = Guid.Parse(targetUserId);
            var currentUserConnection = Guid.NewGuid();
            var targetUserConnection = Guid.NewGuid();

            var connections = new List<UserConnection> 
            { 
                new UserConnection { UserId = currentUserGuid, ConnectedUserId = currentUserConnection },
                new UserConnection { UserId = targetUserGuid, ConnectedUserId = targetUserConnection }
            };

            var mockDbSet = connections
                .AsQueryable()
                .BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r =>
                r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var targetUserIds = new List<string> { targetUserId };
            var result = await _userActionService
                .GetMutualConnectionsCountByUserAsync(currentUserId, targetUserIds);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[targetUserId], Is.EqualTo(0));
        }

        [Test]
        public async Task GetMutualConnectionsCountByUserAsync_WithEmptyTargetList_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var currentUserId = Guid.NewGuid().ToString();
            var targetUserIds = new List<string>();

            var mockDbSet = new List<UserConnection>()
                .AsQueryable()
                .BuildMockDbSet();
            _mockUserConnectionRepository.Setup(r =>
                r.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _userActionService
                .GetMutualConnectionsCountByUserAsync(currentUserId, targetUserIds);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        #endregion

        #region IncrementPostViewsAsync Tests

        [Test]
        public async Task IncrementPostViewsAsync_ShouldIncrementViews_WhenPostExistsAndUserIsDifferent()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var postOwnerId = Guid.NewGuid();
            var post = new Post
            {
                Id = postGuid,
                UserId = postOwnerId,
                Views = 5
            };

            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync(post);

            // Act
            var result = await _userActionService.IncrementPostViewsAsync(postGuid, userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True);
                Assert.That(post.Views, Is.EqualTo(6));
            });
            _mockPostRepository.Verify(r => r.UpdateAsync(post), Times.Once);
            _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task IncrementPostViewsAsync_ShouldNotIncrementViews_WhenPostDoesNotExist()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync((Post)null);

            // Act
            var result = await _userActionService.IncrementPostViewsAsync(postGuid, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockPostRepository.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }

        [Test]
        public async Task IncrementPostViewsAsync_ShouldNotIncrementViews_WhenUserIsPostOwner()
        {
            // Arrange
            var postGuid = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var post = new Post
            {
                Id = postGuid,
                UserId = userGuid,
                Views = 5
            };

            _mockPostRepository.Setup(r => r.GetByIdAsync(postGuid)).ReturnsAsync(post);

            // Act
            var result = await _userActionService.IncrementPostViewsAsync(postGuid, userId);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.False);
                Assert.That(post.Views, Is.EqualTo(5)); // Views should not change
            });
            _mockPostRepository.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }

        #endregion
    }
}