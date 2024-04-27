using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace HealthInsuranceSystem.Api.Security.Authorization
{
    public class RequiresClaimsPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => Task.FromResult(DefaultPolicy.Build());

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            //throw new NotImplementedException();
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(RequiresClaimsAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var claimNames = policyName.Substring(RequiresClaimsAttribute.PolicyPrefix.Length).Trim();
                var claims = claimNames
                    .Split(RequiresClaimsAttribute.Delimiter)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();

                if (claims.Any())
                {
                    return Task.FromResult(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .AddRequirements(new RequiresClaimsRequirement(claims))
                        .Build());
                }
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}
