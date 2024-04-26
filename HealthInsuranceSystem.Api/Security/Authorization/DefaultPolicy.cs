using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace HealthInsuranceSystem.Api.Security.Authorization
{
    public static class DefaultPolicy
    {
        public static AuthorizationPolicy Build() => new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    }
}
