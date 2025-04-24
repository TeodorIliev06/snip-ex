namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Web.ViewModels.DTOs;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Language;
    using SnipEx.Data.Repositories.Contracts;

    public class LanguageService(
        IRepository<ProgrammingLanguage, Guid> languageRepository) : ILanguageService
    {
        public async Task<IEnumerable<LanguageViewModel>> GetAllLanguagesAsync()
        {
            var languages = await languageRepository
                .GetAllAttached()
                .To<LanguageViewModel>()
                .ToListAsync();

            return languages;
        }

        public IEnumerable<LanguageDistributionDto> GetUserPostsLanguagesDistribution(
            IEnumerable<PostCardViewModel> postCards)
        {
            var languageDistribution = postCards
                .GroupBy(p => p.LanguageName)
                .Select(g => 
                    new LanguageDistributionDto { Name = g.Key, Count = g.Count() })
                .OrderByDescending(ls => ls.Count)
                .ToList();

            return languageDistribution;
        }
    }
}
