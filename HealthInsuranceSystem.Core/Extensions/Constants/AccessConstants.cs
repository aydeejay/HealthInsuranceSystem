namespace HealthInsuranceSystem.Core.Extensions.Constants
{
    public static class AccessConstants
    {
        public static class ErrorMessages
        {
            public const string UserExist = "User does not exist";
            public const string FailedLogin = "Authentication failed. Invalid credentials provided";
            public const string AuthFailed = "Feels can't authenticate user at the moment";
        }

        public class Messages
        {
            public const string AuthGlobalKey = "rgkfpjdpjdvpsfjpsj%&$#@!qhjoshooafhohevno1298ydf";
        }
    }

    public static class ValidationConstants
    {
        public const int MaxClientIdLength = 50;
        public const int MaxClientSecretLength = 50;
        public const int MaxEmailLength = 50;
        public const int MaxPasswordLength = 50;
        public const int MaxScopeLength = 50;
    }
}
