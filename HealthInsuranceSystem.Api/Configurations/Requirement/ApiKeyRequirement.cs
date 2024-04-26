using HealthInsuranceSystem.Api.Filter;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthInsuranceSystem.Api.Configurations.Requirement
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
    }

    public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            SucceedRequirementIfApiKeyPresentAndValid(context, requirement);

            if (!context.HasSucceeded)
                context.Fail();

            return Task.CompletedTask;
        }

        private void SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            var apiKeyClaim = context.User?.FindFirst(IdentityClaims.UserClaim);

            if (apiKeyClaim != null && !string.IsNullOrEmpty(apiKeyClaim.Value))
            {
                context.Succeed(requirement);
            }
            else if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                if (authorizationFilterContext.ActionDescriptor is ControllerActionDescriptor descriptor)
                {
                    var attr = descriptor.MethodInfo.GetCustomAttributes(typeof(AcceptsParameterKeyAttribute), true);
                    if (attr == null || attr.Length == 0)
                        attr = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AcceptsParameterKeyAttribute), true);

                    if (attr != null && attr.Length > 0)
                    {
                        var apiKey = authorizationFilterContext.HttpContext.Request.Headers[Constants.Auth.ApiKey].FirstOrDefault();
                        if (apiKey != null)
                            context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
