namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.DTOs;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Web.ViewModels.Language;

    public interface ILanguageService
    {
        Task<IEnumerable<LanguageViewModel>> GetAllLanguagesAsync();

        IEnumerable<LanguageDistributionDto> GetUserPostsLanguagesDistribution(
            IEnumerable<PostCardViewModel> postCards);
    }
}
