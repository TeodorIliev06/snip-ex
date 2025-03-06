namespace SnipEx.Web.ViewModels.Comment
{
    using System.Globalization;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.ApplicationConstants;
    using static Common.EntityValidationConstants.Comment;

    public class CommentViewModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public CommentViewModel()
        {
            this.CreatedAt = DateTime.UtcNow
                .ToString(CreatedAtFormat, CultureInfo.InvariantCulture);
        }

        public string Id { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? UserName { get; set; }
        public string CreatedAt { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentViewModel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UserName, opt =>
                    opt.MapFrom(s => s.User != null ? s.User.UserName : NoUserName));
        }
    }
}
