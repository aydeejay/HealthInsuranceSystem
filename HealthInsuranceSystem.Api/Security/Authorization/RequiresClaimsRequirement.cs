
using HealthInsuranceSystem.Core.Security;

using Microsoft.AspNetCore.Authorization;

namespace HealthInsuranceSystem.Api.Security.Authorization
{
    public class RequiresClaimsRequirement : IAuthorizationRequirement
    {
        public RequiresClaimsRequirement(string[] claims)
        {
            Claims = claims;
        }

        public string[] Claims { get; }
    }

    public class RequiresClaimHandler : AuthorizationHandler<RequiresClaimsRequirement>
    {
        private readonly IIdentityUserContext _identity;

        public RequiresClaimHandler(IIdentityUserContext identity)
        {
            _identity = identity;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiresClaimsRequirement requirement)
        {
            try
            {
                var user = _identity.RequestingUser;
                if (user != null && requirement.Claims.All(x => user.Claims.Any(y => y.Name.Equals(x, StringComparison.OrdinalIgnoreCase))))
                    context.Succeed(requirement);
            }
            catch (AuthorizationFailedException)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }

}
