using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Security
{
    public static class Claims
    {
        public const string CanRequest = "Request Claims";
        public const string AcceptRequest = "Accept Claims";
        public const string EditRequest = "Edit Claims";
        public const string DeclineREquest = "Remove Roles";
        public const string WebPortal = "Web Portal";

    }
}
