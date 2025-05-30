namespace SnipEx.Web.ViewModels.User
{
    using AutoMapper;

    using SnipEx.Data.Models;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Services.Mapping.Contracts;

    public class ConnectionViewModel : IMapFrom<UserConnection>, IHaveCustomMappings
    {
        public string ActorAvatar { get; set; } = null!;

        public int PostsCount { get; set; }

        public ConnectionStatus Type { get; set; }

        public string CssType
        {
            get
            {
                return Type switch
                {
                    ConnectionStatus.Accepted => "connected",
                    _ => "general"
                };
            }
        }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserConnection, ConnectionViewModel>()
                .ForMember(d => d.PostsCount,
                    opt =>
                        opt.MapFrom(uc => uc.ConnectedUser.Posts.Count))
                .ForMember(d => d.ActorAvatar,
                    opt =>
                        opt.MapFrom(uc =>
                            "/" + uc.ConnectedUser.ProfilePicturePath));
        }
    }
}
