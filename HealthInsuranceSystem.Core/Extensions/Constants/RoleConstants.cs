namespace HealthInsuranceSystem.Core.Extensions.Constants
{
    public static class RoleConstants
    {
        public static class ErrorMessages
        {
            public const string RoleNotFoundWithID = "No Role was found with the provided ID.";
            public const string RoleUpdateSuccessful = "Role Updated successfully.";
            public const string RoleSavedSuccessful = "Role saved successfully.";
            public const string RoleIDNotGreaterThanZero = "'Role Id' must be greater than '0'.";
            public const string RoleType = "Custom";
            public const string UserUnableToUpdateUser = "The current User does not have sufficient permissions to alter another user profile.";
            public const string InvalidRole = "Invalid role selected for user.";
        }
    }
}
