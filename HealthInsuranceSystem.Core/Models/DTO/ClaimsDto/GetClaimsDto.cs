namespace HealthInsuranceSystem.Core.Models.DTO.ClaimsDto
{
    public class GetClaimDto
    {
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ExpenseDescription { get; set; }
        public string CurrentStatus { get; set; }
        public int PolicyHolderId { get; set; }
        public int? AssignedUserId { get; set; }
        public string? CurrentComment { get; set; }
    }
}
