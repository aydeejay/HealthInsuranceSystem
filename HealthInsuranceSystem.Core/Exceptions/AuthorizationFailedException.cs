using System.Security;

namespace HealthInsuranceSystem.Core.Exceptions
{
    public class AuthorizationFailedException : SecurityException
    {
        public AuthorizationFailedException()
        {
        }

        public AuthorizationFailedException(string message) : base(message)
        {
        }
    }
}
