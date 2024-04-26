using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Models.Domain.Authorization
{
    public class Role
    {
        public int RoleId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string RoleType { get; set; }

        public ICollection<RoleAuthClaim> RoleClaims { get; set; }

        public ICollection<User> Users { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public Role()
        {
            RoleClaims = new List<RoleAuthClaim>();
        }
    }
}
