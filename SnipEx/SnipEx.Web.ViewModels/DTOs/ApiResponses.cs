namespace SnipEx.Web.ViewModels.DTOs
{
    public class ApiResponses
    {
        public class TogglePostLikeResponse
        {
            public bool IsLiked { get; set; }
            public int LikesCount { get; set; }
        }

        public class TogglePostSaveResponse
        {
            public bool IsSaved { get; set; }
        }

        public class ToggleCommentLikeResponse
        {
            public bool IsLiked { get; set; }
            public int LikesCount { get; set; }
        }

        public class ToggleConnectionResponse
        {
            public bool IsConnected { get; set; }
            public int ConnectionsCount { get; set; }
        }
    }
}
