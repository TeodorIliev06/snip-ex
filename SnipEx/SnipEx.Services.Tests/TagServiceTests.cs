namespace SnipEx.Services.Tests
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable.Moq;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Services.Data.Models;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Data.Repositories.Contracts;

    [TestFixture]
    public class TagServiceTests
    {
        private Mock<IRepository<Tag, Guid>> _mockTagRepository;
        private Mock<IRepository<PostTag, object>> _mockPostTagRepository;
        private TagService _tagService;

        [SetUp]
        public void Setup()
        {
            _mockTagRepository = new Mock<IRepository<Tag, Guid>>();
            _mockPostTagRepository = new Mock<IRepository<PostTag, object>>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _tagService = new TagService(_mockTagRepository.Object, _mockPostTagRepository.Object);
        }

        [Test]
        public async Task GetTrendingTagsAsync_ShouldReturnTags_WhenTagsExist()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Id = Guid.NewGuid(), Name = "C#" },
                new Tag { Id = Guid.NewGuid(), Name = "JavaScript" },
                new Tag { Id = Guid.NewGuid(), Name = "Python" }
            };

            var mockDbSet = tags.AsQueryable().BuildMockDbSet();
            _mockTagRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _tagService.GetTrendingTagsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));

            var resultList = result.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(resultList[0].Name, Is.EqualTo("C#"));
                Assert.That(resultList[1].Name, Is.EqualTo("JavaScript"));
                Assert.That(resultList[2].Name, Is.EqualTo("Python"));
            });

            _mockTagRepository
                .Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task AddTagsToPostAsync_ShouldReturnFalse_WhenModelsIsNull()
        {
            // Arrange
            IEnumerable<AddTagFormModel> models = null;
            var postGuid = Guid.NewGuid();

            // Act
            var result = await _tagService.AddTagsToPostAsync(models, postGuid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddTagsToPostAsync_ShouldReturnFalse_WhenModelsIsEmpty()
        {
            // Arrange
            var models = new List<AddTagFormModel>();
            var postGuid = Guid.NewGuid();

            // Act
            var result = await _tagService.AddTagsToPostAsync(models, postGuid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddTagsToPostAsync_ShouldReturnFalse_WhenAllTagNamesAreEmpty()
        {
            // Arrange
            var models = new List<AddTagFormModel>
            {
                new AddTagFormModel { Name = "" },
                new AddTagFormModel { Name = "   " },
                new AddTagFormModel { Name = null }
            };
            var postGuid = Guid.NewGuid();

            // Act
            var result = await _tagService.AddTagsToPostAsync(models, postGuid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddTagsToPostAsync_ShouldCreateNewTags_WhenTagsDoNotExist()
        {
            // Arrange
            var models = new List<AddTagFormModel>
            {
                new AddTagFormModel { Name = "NewTag1" },
                new AddTagFormModel { Name = "NewTag2" }
            };
            var postGuid = Guid.NewGuid();

            var existingTags = new List<Tag>();
            var mockExistingTagsDbSet = existingTags.AsQueryable().BuildMockDbSet();

            _mockTagRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockExistingTagsDbSet.Object);

            _mockTagRepository
                .Setup(r => 
                    r.AddRangeAsync(It.IsAny<Tag[]>()))
                .Returns(Task.CompletedTask);

            _mockTagRepository
                .Setup(r =>
                    r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockPostTagRepository
                .Setup(r =>
                    r.AddRangeAsync(It.IsAny<PostTag[]>()))
                .Returns(Task.CompletedTask);

            _mockPostTagRepository
                .Setup(r =>
                    r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tagService.AddTagsToPostAsync(models, postGuid);

            // Assert
            Assert.That(result, Is.True);
            _mockTagRepository
                .Verify(r =>
                    r.AddRangeAsync(It.IsAny<Tag[]>()), Times.Once);

            _mockTagRepository
                .Verify(r =>
                    r.SaveChangesAsync(), Times.Once);

            _mockPostTagRepository
                .Verify(r =>
                    r.AddRangeAsync(It.IsAny<PostTag[]>()), Times.Once);

            _mockPostTagRepository
                .Verify(r =>
                    r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddTagsToPostAsync_ShouldNotCreateDuplicateTags_WhenTagsAlreadyExist()
        {
            // Arrange
            var existingTagId = Guid.NewGuid();
            var models = new List<AddTagFormModel>
            {
                new AddTagFormModel { Name = "ExistingTag" }
            };
            var postGuid = Guid.NewGuid();

            var existingTags = new List<Tag>
            {
                new Tag { Id = existingTagId, Name = "ExistingTag" }
            };

            var mockExistingTagsDbSet = existingTags.AsQueryable().BuildMockDbSet();

            _mockTagRepository
                .Setup(r =>
                    r.GetAllAttached())
                .Returns(mockExistingTagsDbSet.Object);

            _mockPostTagRepository
                .Setup(r =>
                    r.AddRangeAsync(It.IsAny<PostTag[]>()))
                .Returns(Task.CompletedTask);

            _mockPostTagRepository
                .Setup(r =>
                    r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tagService.AddTagsToPostAsync(models, postGuid);

            // Assert
            Assert.That(result, Is.True);

            _mockTagRepository
                .Verify(r =>
                    r.AddRangeAsync(It.IsAny<Tag[]>()), Times.Never);

            _mockTagRepository
                .Verify(r =>
                    r.SaveChangesAsync(), Times.Never);

            _mockPostTagRepository
                .Verify(r =>
                    r.AddRangeAsync(It.IsAny<PostTag[]>()), Times.Once);

            _mockPostTagRepository
                .Verify(r =>
                    r.SaveChangesAsync(), Times.Once);
        }

    }
}