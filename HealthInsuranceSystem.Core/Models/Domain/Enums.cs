namespace HealthInsuranceSystem.Core.Models.Domain
{
    public enum ClaimStatus
    {
        Submitted = 0,
        Approved = 1,
        Declined = 2,
        InReview = 3,
    }
    public enum RoleType
    {
        Unassigned = 0,
        Administrator = 1,
        ClaimProcessor = 2,
        PolicyHolder = 3,
    }
}