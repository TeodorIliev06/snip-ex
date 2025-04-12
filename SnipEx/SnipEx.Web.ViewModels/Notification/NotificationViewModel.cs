namespace SnipEx.Web.ViewModels.Notification
{
    using System.Globalization;

    using AutoMapper;

    using SnipEx.Data.Models;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.Notification;

    public class NotificationViewModel : IMapFrom<Notification>, IHaveCustomMappings
    {
        public string Id { get; set; } = null!;

        public string ActorUsername { get; set; } = null!;

        public string ActorAvatar { get; set; } = null!;

        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string CreatedAt { get; set; } = null!;

        public bool IsRead { get; set; }

        //public string RelatedItemId { get; set; }
        //public string RelatedLink { get; set; }

        public string RelatedEntityType { get; set; } = null!;

        public string CssType
        {
            get
            {
                return Type switch
                {
                    NotificationType.PostLike or NotificationType.CommentLike => "like",
                    NotificationType.PostComment or NotificationType.CommentReply => "comment",
                    NotificationType.Mention => "mention",
                    _ => "general"
                };
            }
        }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Notification, NotificationViewModel>()
                .ForMember(d => d.CreatedAt,
                    opt =>
                        opt.MapFrom(n => n.CreatedAt.ToString(CreatedAtFormat, CultureInfo.InvariantCulture)))
                .ForMember(d => d.ActorAvatar,
                    opt =>
                        opt.MapFrom(n => "/" + n.Actor.ProfilePicturePath));
        }
    }
}
