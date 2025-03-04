namespace SnipEx.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;

    public class TagService(
        IRepository<Tag, Guid> tagRepository) : ITagService
    {
        public async Task<IEnumerable<TagViewModel>> GetTrendingTagsAsync()
        {
            var trendingTags = await tagRepository
                .GetAllAttached()
                .To<TagViewModel>()
                .Take(20)
                .ToListAsync();

            return trendingTags;
        }
    }
}
