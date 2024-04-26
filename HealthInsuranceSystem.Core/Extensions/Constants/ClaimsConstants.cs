namespace HealthInsuranceSystem.Core.Extensions.Constants
{
    public static class ClaimConstants
    {
        public static class Messages
        {
            public const string ClaimsCreatedSuccessful = "Claim created Successfully";
            public const string ClaimsUpdateSuccessful = "Claim updated Successfully";

        }
        public static class ErrorMessages
        {
            public const string ClaimsFailed = "Error saving Claim";
            public const string ClaimsUpdateFailed = "Error updating Claim";
            public const string ClaimsNotFound = "Claims does not exist";
        }
    }
}
