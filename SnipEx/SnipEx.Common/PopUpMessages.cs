namespace SnipEx.Common
{
    using static EntityValidationConstants;

    public class PopUpMessages
    {
        public static class PopUpError
        {
            public static string InvalidCommentLength = $"Comment must be between {Comment.ContentMinLength} and {Comment.ContentMaxLength} characters long.";
            public static string InvalidOperation = "Your comment could not be added. Please contact an administrator.";
        }

        public const string ErrorMessage = "ErrorMessage";
        public const string WarningMessage = "WarningMessage";
        public const string SuccessMessage = "SuccessMessage";
        public const string InfoMessage = "InfoMessage";
    }
}
