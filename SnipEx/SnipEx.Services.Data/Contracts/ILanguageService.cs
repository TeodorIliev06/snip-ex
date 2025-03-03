namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Language;

    public interface ILanguageService
    {
        Task<IEnumerable<LanguageViewModel>> GetAllLanguagesAsync();
    }
}
