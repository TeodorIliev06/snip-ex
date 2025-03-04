namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Tag;

    public interface ITagService
    {
        Task<IEnumerable<TagViewModel>> GetTrendingTagsAsync();
    }
}
