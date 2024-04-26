using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Extensions.Constants
{
    public static class Constants
    {
        public static class Auth
        {
            public const string UserPolicy = "User-policy";
            public const string ApiKey = "api-key";
            public const string NoUser = "No users found for this request.";
            public const string UnauthorizedUser = "User is not Authorized to access this resource";
        }
    }
}
