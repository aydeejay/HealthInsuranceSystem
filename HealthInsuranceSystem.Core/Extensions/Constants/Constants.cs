namespace HealthInsuranceSystem.Core.Extensions.Constants
{
    public static class Constants
    {
        public const int MaxPageSize = 100;
        public static class Auth
        {
            public const string UserPolicy = "User-policy";
            public const string ApiKey = "api-key";
            public const string NoUser = "No users found for this request.";
            public const string UnauthorizedUser = "User is not Authorized to access this resource";
        }
    }
}
