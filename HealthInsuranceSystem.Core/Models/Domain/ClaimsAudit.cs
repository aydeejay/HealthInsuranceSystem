using System.ComponentModel.DataAnnotations.Schema;

namespace HealthInsuranceSystem.Core.Models.Domain
{
    public class ClaimsAudit
    {
        public int Id { get; set; }
        public int PolicyHolderId { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Comment { get; set; }
        public string? OldStatus { get; set; }
        public string NewStatus { get; set; }

        [ForeignKey("AssignedUserId")]
        public User AssignedUser { get; set; }
        [ForeignKey("PolicyHolderId")]
        public User PolicyHolder { get; set; }
    }
}