namespace SnipEx.Web.ViewModels.Post
{
    using System.Globalization;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Post;
    using static Common.ApplicationConstants;

    public class PostCardViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public PostCardViewModel()
        {
            this.CreatedAt = DateTime.UtcNow
                .ToString(CreatedAtFormat, CultureInfo.InvariantCulture);
        }

        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string CreatedAt { get; set; }

        public string? UserName { get; set; }

        public string LanguageName { get; set; } = null!;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Post, PostCardViewModel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UserName, opt =>
                    opt.MapFrom(s => s.User != null ? s.User.UserName : NoUserName));
        }
    }
}
