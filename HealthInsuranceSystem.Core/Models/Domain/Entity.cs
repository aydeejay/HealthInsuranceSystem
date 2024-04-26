using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Models.Domain
{
    public class Entity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
