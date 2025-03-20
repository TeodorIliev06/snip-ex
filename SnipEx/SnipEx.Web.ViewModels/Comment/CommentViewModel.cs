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
        public string Id { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? UserName { get; set; }

        public string CreatedAt { get; set; } = null!;

        public int LikesCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }

        public string PostId { get; set; } = null!;

        public string? ParentCommentId { get; set; }

        public ICollection<CommentViewModel> Replies { get; set; } 
            = new List<CommentViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentViewModel>()
                .ForMember(d => d.CreatedAt,
                    opt =>
                    opt.MapFrom(c => c.CreatedAt.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)))
                .ForMember(d => d.UserName, opt =>
                    opt.MapFrom(s => s.User != null ? s.User.UserName : NoUserName))
                .ForMember(d => d.Replies,
                    opt => opt.MapFrom(c => c.Replies.OrderBy(c => c.CreatedAt).ToList()))
                .ForMember(d => d.IsLikedByCurrentUser,
                    opt => opt.Ignore()); ;
        }
    }
}
