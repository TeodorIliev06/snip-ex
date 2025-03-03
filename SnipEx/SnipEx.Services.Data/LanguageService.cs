namespace SnipEx.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Language;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mapping;

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
    }
}
