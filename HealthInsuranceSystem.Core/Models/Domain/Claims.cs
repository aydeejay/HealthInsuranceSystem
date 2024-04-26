using System.ComponentModel.DataAnnotations.Schema;

namespace HealthInsuranceSystem.Core.Models.Domain
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ExpenseDescription { get; set; }
        public string CurrentStatus { get; set; }
        public int PolicyHolderId { get; set; }
        public int? AssignedUserId { get; set; }
        public string? CurrentComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        [ForeignKey("AssignedUserId")]
        public User AssignedUser { get; set; }

        [ForeignKey("PolicyHolderId")]
        public User PolicyHolder { get; set; }
    }
}
