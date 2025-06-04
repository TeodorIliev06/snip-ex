namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Tag;

    public interface ITagService
    {
        Task<IEnumerable<TagViewModel>> GetTrendingTagsAsync();

        Task<bool> AddTagsToPostAsync(IEnumerable<AddTagFormModel> models, Guid postGuid);
    }
}
