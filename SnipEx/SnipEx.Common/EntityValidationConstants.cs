namespace SnipEx.Common
{
    public static class EntityValidationConstants
    {
        public static class User
        {
            public const int UsernameMinLength = 1;
            public const int UsernameMaxLength = 100;
        }

        public static class Comment
        {
            public const int ContentMinLength = 50;
            public const int ContentMaxLength = 2000;
            public const string CreatedAtFormat = "dd/MM/yyyy";
        }

        public static class Post
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 70;
            public const int ContentMinLength = 50;
            public const int ContentMaxLength = 5000;
            public const int ViewMinValue = 0;
            public const int ViewMaxValue = 1000; //Come up with a way to display 1000+ views
            public const string RatingMinValue = "1";
            public const string RatingMaxValue = "5";
            public const string CreatedAtFormat = "dd/MM/yyyy";
        }

        public static class ProgrammingLanguage
        {
            public const int NameMinLength = 1;
            public const int NameMaxLength = 40;
            public const int FileExtensionMinLength = 2;
            public const int FileExtensionMaxLength = 10;
        }

        public static class Tag
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 50;
            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 500;
        }

        public static class Notification
        {
            public const int MessageMinLength = 5;
            public const int MessageMaxLength = 200;
            public const int EntityTypeMinLength = 2;
            public const int EntityTypeMaxLength = 50;
        }
    }
}
