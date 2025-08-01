﻿namespace SnipEx.Services.Tests
{
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(4));

            var resultList = result.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(resultList[0].Name, Is.EqualTo("C#"));
                Assert.That(resultList[1].Name, Is.EqualTo("JavaScript"));
                Assert.That(resultList[2].Name, Is.EqualTo("Python"));
                Assert.That(resultList[3].Name, Is.EqualTo("Java"));
            });

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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));

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
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(3));

            Assert.Multiple(() =>
            {
                // Should be ordered by count descending
                Assert.That(resultList[0].Name, Is.EqualTo("C#"));
                Assert.That(resultList[0].Count, Is.EqualTo(3));

                Assert.That(resultList[1].Name, Is.EqualTo("JavaScript"));
                Assert.That(resultList[1].Count, Is.EqualTo(2));

                Assert.That(resultList[2].Name, Is.EqualTo("Python"));
                Assert.That(resultList[2].Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void GetUserPostsLanguagesDistribution_ShouldReturnEmptyCollection_WhenPostCardsIsEmpty()
        {
            // Arrange
            var postCards = new List<PostCardViewModel>();

            // Act
            var result = _languageService.GetUserPostsLanguagesDistribution(postCards);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
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
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);

                var resultList = result.ToList();
                Assert.That(resultList.Count, Is.EqualTo(1));
                Assert.That(resultList[0].Name, Is.EqualTo("C#"));
                Assert.That(resultList[0].Count, Is.EqualTo(3));
            });
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
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(3));

            // Verify that null is treated as a separate group
            var nullGroup = resultList.FirstOrDefault(r => r.Name == null);
            Assert.That(nullGroup, Is.Not.Null);
            Assert.That(nullGroup.Count, Is.EqualTo(2));

            var csharpGroup = resultList.FirstOrDefault(r => r.Name == "C#");
            Assert.That(csharpGroup, Is.Not.Null);
            Assert.That(csharpGroup.Count, Is.EqualTo(1));

            var jsGroup = resultList.FirstOrDefault(r => r.Name == "JavaScript");
            Assert.That(jsGroup, Is.Not.Null);
            Assert.That(jsGroup.Count, Is.EqualTo(1));
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
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(4));

            // All should have count of 1
            Assert.That(resultList.All(r => r.Count == 1));

            // Verify that they are still properly grouped and ordered
            var languages = resultList.Select(r => r.Name).ToList();
            Assert.That(languages, Does.Contain("C#"));
            Assert.That(languages, Does.Contain("JavaScript"));
            Assert.That(languages, Does.Contain("Python"));
            Assert.That(languages, Does.Contain("Java"));
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
            Assert.That(result, Is.Not.Null);
            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(3));

            Assert.Multiple(() =>
            {
                Assert.That(resultList[0].Name, Is.EqualTo("C#"));
                Assert.That(resultList[0].Count, Is.EqualTo(500));

                Assert.That(resultList[1].Name, Is.EqualTo("JavaScript"));
                Assert.That(resultList[1].Count, Is.EqualTo(300));

                Assert.That(resultList[2].Name, Is.EqualTo("Python"));
                Assert.That(resultList[2].Count, Is.EqualTo(200));
            });
        }
    }
}