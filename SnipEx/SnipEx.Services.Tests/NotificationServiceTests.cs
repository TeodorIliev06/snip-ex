namespace SnipEx.Services.Tests
{
    using Moq;
    using MockQueryable;
    using NUnit.Framework;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Services.Data.Models;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Data.Repositories.Contracts;

    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<IRepository<Notification, Guid>> _mockNotificationRepository;
        private NotificationService _notificationService;

        [SetUp]
        public void Setup()
        {
            _mockNotificationRepository = new Mock<IRepository<Notification, Guid>>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _notificationService = new NotificationService(_mockNotificationRepository.Object);
        }

        #region GetUserNotificationsAsync Tests

        [Test]
        public async Task GetUserNotificationsAsync_ShouldReturnNotifications_WhenValidUserId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var actorGuid = Guid.NewGuid();

            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "Test notification 1",
                    Type = NotificationType.PostLike,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                    RecipientId = userGuid,
                    ActorId = actorGuid,
                    RelatedEntityId = Guid.NewGuid(),
                    RelatedEntityType = "Post",
                    Recipient = new ApplicationUser { Id = userGuid, UserName = "recipient" },
                    Actor = new ApplicationUser { Id = actorGuid, UserName = "actor", ProfilePicturePath = "images/actor.jpg" }
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "Test notification 2",
                    Type = NotificationType.CommentReply,
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    RecipientId = userGuid,
                    ActorId = actorGuid,
                    RelatedEntityId = Guid.NewGuid(),
                    RelatedEntityType = "Comment",
                    Recipient = new ApplicationUser { Id = userGuid, UserName = "recipient" },
                    Actor = new ApplicationUser { Id = actorGuid, UserName = "actor", ProfilePicturePath = "images/actor.jpg" }
                }
            };

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetUserNotificationsAsync(userId, 1, 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var firstNotification = result.First();
            Assert.That(firstNotification.Message, Is.EqualTo("Test notification 1"));
            Assert.That(firstNotification.Type, Is.EqualTo(NotificationType.PostLike));
            Assert.That(firstNotification.IsRead, Is.False);
            Assert.That(firstNotification.ActorAvatar, Is.EqualTo("/images/actor.jpg"));
        }

        [Test]
        public async Task GetUserNotificationsAsync_ShouldReturnEmpty_WhenNoNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var notifications = new List<Notification>();

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetUserNotificationsAsync(userId, 1, 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetUserNotificationsAsync_ShouldRespectPagination_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var actorGuid = Guid.NewGuid();

            var notifications = new List<Notification>();
            for (int i = 0; i < 25; i++)
            {
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = $"Test notification {i}",
                    Type = NotificationType.PostLike,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    RecipientId = userGuid,
                    ActorId = actorGuid,
                    RelatedEntityId = Guid.NewGuid(),
                    RelatedEntityType = "Post",
                    Recipient = new ApplicationUser { Id = userGuid, UserName = "recipient" },
                    Actor = new ApplicationUser { Id = actorGuid, UserName = "actor", ProfilePicturePath = "images/actor.jpg" }
                });
            }

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetUserNotificationsAsync(userId, 2, 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(10));
        }

        [Test]
        public void GetUserNotificationsAsync_ShouldThrowException_WhenInvalidUserId()
        {
            // Arrange
            var invalidUserId = "invalid-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _notificationService.GetUserNotificationsAsync(invalidUserId));
        }

        #endregion

        #region GetUnreadNotificationsCountAsync Tests

        [Test]
        public async Task GetUnreadNotificationsCountAsync_ShouldReturnCorrectCount_WhenUnreadNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var notifications = new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true },
                new Notification { Id = Guid.NewGuid(), RecipientId = Guid.NewGuid(), IsRead = false } // Different user
            };

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetUnreadNotificationsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetUnreadNotificationsCountAsync_ShouldReturnZero_WhenNoUnreadNotifications()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var notifications = new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true }
            };

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetUnreadNotificationsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetUnreadNotificationsCountAsync_ShouldThrowException_WhenInvalidUserId()
        {
            // Arrange
            var invalidUserId = "invalid-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _notificationService.GetUnreadNotificationsCountAsync(invalidUserId));
        }

        #endregion

        #region GetTotalNotificationsCountAsync Tests

        [Test]
        public async Task GetTotalNotificationsCountAsync_ShouldReturnCorrectCount_WhenNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var notifications = new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false },
                new Notification { Id = Guid.NewGuid(), RecipientId = Guid.NewGuid(), IsRead = false } // Different user
            };

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetTotalNotificationsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task GetTotalNotificationsCountAsync_ShouldReturnZero_WhenNoNotifications()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var notifications = new List<Notification>();

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.GetTotalNotificationsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion

        #region MarkNotificationAsReadAsync Tests

        [Test]
        public async Task MarkNotificationAsReadAsync_ShouldReturnTrue_WhenNotificationExistsAndBelongsToUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);
            var notificationGuid = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notificationGuid,
                RecipientId = userGuid,
                IsRead = false
            };

            _mockNotificationRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(notification);
            _mockNotificationRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationGuid, userId);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(notification.IsRead, Is.True);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task MarkNotificationAsReadAsync_ShouldReturnFalse_WhenNotificationDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var notificationGuid = Guid.NewGuid();

            _mockNotificationRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
                .ReturnsAsync((Notification?)null);

            // Act
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationGuid, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkNotificationAsReadAsync_ShouldReturnFalse_WhenNotificationBelongsToDifferentUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var differentUserGuid = Guid.NewGuid();
            var notificationGuid = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notificationGuid,
                RecipientId = differentUserGuid,
                IsRead = false
            };

            _mockNotificationRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
                .ReturnsAsync((Notification?)null); // Repository should filter by user, so it returns null

            // Act
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationGuid, userId);

            // Assert
            Assert.That(result, Is.False);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region MarkAllNotificationsAsReadAsync Tests

        [Test]
        public async Task MarkAllNotificationsAsReadAsync_ShouldReturnTrue_WhenUnreadNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var unreadNotifications = new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = false }
            };

            var mockQueryable = unreadNotifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
            _mockNotificationRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(unreadNotifications.All(n => n.IsRead), Is.True);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task MarkAllNotificationsAsReadAsync_ShouldReturnFalse_WhenNoUnreadNotifications()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var userGuid = Guid.Parse(userId);

            var readNotifications = new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true },
                new Notification { Id = Guid.NewGuid(), RecipientId = userGuid, IsRead = true }
            };

            // Filter to return empty list for unread notifications
            var emptyQueryable = new List<Notification>().AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(emptyQueryable);

            // Act
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            // Assert
            Assert.That(result, Is.False);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkAllNotificationsAsReadAsync_ShouldReturnFalse_WhenNoNotificationsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var notifications = new List<Notification>();

            var mockQueryable = notifications.AsQueryable().BuildMock();
            _mockNotificationRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            // Assert
            Assert.That(result, Is.False);
            _mockNotificationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public void MarkAllNotificationsAsReadAsync_ShouldThrowException_WhenInvalidUserId()
        {
            // Arrange
            var invalidUserId = "invalid-guid";

            // Act & Assert
            Assert.ThrowsAsync<FormatException>(async () =>
                await _notificationService.MarkAllNotificationsAsReadAsync(invalidUserId));
        }

        #endregion
    }
}