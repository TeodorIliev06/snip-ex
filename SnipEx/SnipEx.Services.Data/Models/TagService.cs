namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    public class TagService(
        IRepository<Tag, Guid> tagRepository,
        IRepository<PostTag, object> postTagRepository) : ITagService
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

        public async Task<bool> AddTagsToPostAsync(IEnumerable<AddTagFormModel> models, Guid postGuid)
        {
            if (models == null || !models.Any())
            {
                return false;
            }

            var tagNames = models
                .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                .Select(m => m.Name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!tagNames.Any())
                return false;

            // Fetch tags to exclude 
            var existingTags = await tagRepository
                .GetAllAttached()
                .Where(t => tagNames.Contains(t.Name))
                .ToListAsync();

            var existingNames = existingTags
                .Select(t => t.Name.ToLower())
                .ToHashSet();

            //Sidenote: comparison is made with case-insensitive distinction
            var newTags = models
                .Where(m => 
                    !string.IsNullOrWhiteSpace(m.Name) &&
                    !existingNames.Contains(m.Name.Trim().ToLower()))
                .DistinctBy(m => m.Name.Trim().ToLower())
                .Select(m => {
                    var tag = new Tag();
                    AutoMapperConfig.MapperInstance.Map(m, tag);
                    return tag;
                })
                .ToList();

            if (newTags.Any())
            {
                await tagRepository.AddRangeAsync(newTags.ToArray());
                await tagRepository.SaveChangesAsync();
            }

            var allTags = existingTags.Concat(newTags);
            var postTags = allTags
                .Select(tag => new PostTag { PostId = postGuid, TagId = tag.Id })
                .ToArray();

            await postTagRepository.AddRangeAsync(postTags);
            await postTagRepository.SaveChangesAsync();

            return true;
        }
    }
}
