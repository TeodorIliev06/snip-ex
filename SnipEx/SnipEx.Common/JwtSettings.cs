namespace SnipEx.Common
{
    public static class JwtSettings
    {
        public const string Key = "SnipExSuperSecretKey_LongEnoughForSHA256";
        public const string Issuer = "SnipEx.Web";
        public const string Audience = "SnipEx.WebApi";
        public const int ExpiryMinutes = 60;
    }
}
