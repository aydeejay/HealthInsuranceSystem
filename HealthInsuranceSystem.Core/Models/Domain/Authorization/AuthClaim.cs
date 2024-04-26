using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Models.Domain.Authorization
{
    public class AuthClaim
    {
        public int ClaimId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<RoleAuthClaim> RoleAuthClaims { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public AuthClaim()
        {
            RoleAuthClaims = new List<RoleAuthClaim>();
        }
    }
}
