using HealthInsuranceSystem.Core.Models.Domain.Authorization;

using System.ComponentModel.DataAnnotations.Schema;

namespace HealthInsuranceSystem.Core.Models.Domain
{
    public class User : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string UserPolicyNumber { get; set; }
        public string NationalID { get; set; }
        public string Salt { get; set; }
        public string HashPassword { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
