using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthInsuranceSystem.Core.Models.Domain.Authorization
{
    public class RoleAuthClaim
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleClaimId { get; set; }
        public bool Active { get; set; }
        public int AuthClaimId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [ForeignKey("AuthClaimId")]
        public AuthClaim AuthClaim { get; set; }
    }
}
