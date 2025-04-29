namespace SnipEx.Common
{
    using static EntityValidationConstants;

    public class PopUpMessages
    {
        public static class PopUpError
        {
            public static string InvalidCommentLength = $"Comment must be between {Comment.ContentMinLength} and {Comment.ContentMaxLength} characters long.";
            public static string InvalidCommentOperation = "Your comment could not be added. Please contact an administrator.";
            public static string NonExistingPost = "The specified post could not be found. Please check the URL and try again.";
            public static string NonExistingComment = "The specified comment could not be found. Please check the URL and try again.";
            public static string NonExistingUser = "No user could be found. Please check the URL and try again.";
        }

        public const string ErrorMessage = "ErrorMessage";
        public const string WarningMessage = "WarningMessage";
        public const string SuccessMessage = "SuccessMessage";
        public const string InfoMessage = "InfoMessage";
    }
}
