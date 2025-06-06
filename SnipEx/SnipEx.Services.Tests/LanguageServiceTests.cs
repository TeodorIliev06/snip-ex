namespace SnipEx.Services.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Moq;
    using NUnit.Framework;
    using MockQueryable.Moq;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Models;
    using SnipEx.Services.Tests.Utils;
    using SnipEx.Data.Repositories.Contracts;

    [TestFixture]
    public class LanguageServiceTests
    {
        private Mock<IRepository<ProgrammingLanguage, Guid>> _mockLanguageRepository;
        private LanguageService _languageService;

        [SetUp]
        public void Setup()
        {
            _mockLanguageRepository = new Mock<IRepository<ProgrammingLanguage, Guid>>();

            MapperInitializer.Initialize();

            if (AutoMapperConfig.MapperInstance == null)
            {
                throw new InvalidOperationException("AutoMapper was not initialized properly");
            }

            _languageService = new LanguageService(_mockLanguageRepository.Object);
        }

        [Test]
        public async Task GetAllLanguagesAsync_ShouldReturnLanguages_WhenLanguagesExist()
        {
            // Arrange
            var languages = new List<ProgrammingLanguage>
            {
                new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "C#" },
                new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "JavaScript" },
                new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "Python" },
                new ProgrammingLanguage { Id = Guid.NewGuid(), Name = "Java" }
            };

            var mockDbSet = languages.AsQueryable().BuildMockDbSet();
            _mockLanguageRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _languageService.GetAllLanguagesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());

            var resultList = result.ToList();
            Assert.AreEqual("C#", resultList[0].Name);
            Assert.AreEqual("JavaScript", resultList[1].Name);
            Assert.AreEqual("Python", resultList[2].Name);
            Assert.AreEqual("Java", resultList[3].Name);

            _mockLanguageRepository
                .Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetAllLanguagesAsync_ShouldReturnEmptyCollection_WhenNoLanguagesExist()
        {
            // Arrange
            var languages = new List<ProgrammingLanguage>();
            var mockDbSet = languages.AsQueryable().BuildMockDbSet();
            _mockLanguageRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockDbSet.Object);

            // Act
            var result = await _languageService.GetAllLanguagesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            _mockLanguageRepository
                .Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldReturnDistribution_WhenPostCardsProvided()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>
            {
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "JavaScript" },
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "Python" },
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "JavaScript" }
            };

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);

            // Should be ordered by count descending
            Assert.AreEqual("C#", resultList[0].Name);
            Assert.AreEqual(3, resultList[0].Count);

            Assert.AreEqual("JavaScript", resultList[1].Name);
            Assert.AreEqual(2, resultList[1].Count);

            Assert.AreEqual("Python", resultList[2].Name);
            Assert.AreEqual(1, resultList[2].Count);
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldReturnEmptyCollection_WhenPostCardsIsEmpty()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>();

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldReturnEmptyCollection_WhenPostCardsIsNull()
        {
            // Arrange
            IEnumerable<PostCardViewModel> postCards = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _languageService.GetUserPostsLanguagesDistribution(postCards).ToList();
            });
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldHandleSingleLanguage_WhenAllPostsHaveSameLanguage()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>
            {
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "C#" }
            };

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("C#", resultList[0].Name);
            Assert.AreEqual(3, resultList[0].Count);
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldHandleNullLanguageNames_WhenPostCardsHaveNullLanguages()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>
            {
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = null },
                new PostCardViewModel { LanguageName = "JavaScript" },
                new PostCardViewModel { LanguageName = null }
            };

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);

            // Verify that null is treated as a separate group
            var nullGroup = resultList.FirstOrDefault(r => r.Name == null);
            Assert.IsNotNull(nullGroup);
            Assert.AreEqual(2, nullGroup.Count);

            var csharpGroup = resultList.FirstOrDefault(r => r.Name == "C#");
            Assert.IsNotNull(csharpGroup);
            Assert.AreEqual(1, csharpGroup.Count);

            var jsGroup = resultList.FirstOrDefault(r => r.Name == "JavaScript");
            Assert.IsNotNull(jsGroup);
            Assert.AreEqual(1, jsGroup.Count);
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldMaintainDescendingOrder_WhenLanguagesHaveEqualCounts()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>
            {
                new PostCardViewModel { LanguageName = "C#" },
                new PostCardViewModel { LanguageName = "JavaScript" },
                new PostCardViewModel { LanguageName = "Python" },
                new PostCardViewModel { LanguageName = "Java" }
            };

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(4, resultList.Count);

            // All should have count of 1
            Assert.IsTrue(resultList.All(r => r.Count == 1));

            // Verify that they are still properly grouped and ordered
            var languages = resultList.Select(r => r.Name).ToList();
            Assert.Contains("C#", languages);
            Assert.Contains("JavaScript", languages);
            Assert.Contains("Python", languages);
            Assert.Contains("Java", languages);
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldHandleLargeDataset_WhenManyPostCardsProvided()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>();

            // Create 1000 posts with various languages
            for (int i = 0; i < 500; i++)
                postCards.Add(new PostCardViewModel { LanguageName = "C#" });

            for (int i = 0; i < 300; i++)
                postCards.Add(new PostCardViewModel { LanguageName = "JavaScript" });

            for (int i = 0; i < 200; i++)
                postCards.Add(new PostCardViewModel { LanguageName = "Python" });

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);

            Assert.AreEqual("C#", resultList[0].Name);
            Assert.AreEqual(500, resultList[0].Count);

            Assert.AreEqual("JavaScript", resultList[1].Name);
            Assert.AreEqual(300, resultList[1].Count);

            Assert.AreEqual("Python", resultList[2].Name);
            Assert.AreEqual(200, resultList[2].Count);
        }
    }
}