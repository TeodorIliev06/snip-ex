namespace SnipEx.Web.ViewModels.User
{
    using SnipEx.Data.Models.Enums;

    public class ConnectionViewModel
    {
        public string UserId { get; set; } = null!;

        public string ConnectedUserId { get; set; } = null!;

        public string TargetUserId { get; set; } = null!;

        public string ActorAvatar { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int PostsCount { get; set; }

        public int LikesCount { get; set; }

        public int MutualConnectionsCount { get; set; }

        public ConnectionStatus Type { get; set; }

        public string CssType
        {
            get
            {
                return Type switch
                {
                    ConnectionStatus.Accepted => "connected",
                    ConnectionStatus.Mutual => "mutual",
                    _ => "general"
                };
            }
        }
    }
}
