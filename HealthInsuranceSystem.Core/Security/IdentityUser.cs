using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.RoleClaimsDto;

namespace HealthInsuranceSystem.Core.Security
{
    public class IdentityUser : User
    {
        public string RoleName { get; set; }
        public List<ClaimDto> Claims { get; set; } = new List<ClaimDto>();
    }
}
