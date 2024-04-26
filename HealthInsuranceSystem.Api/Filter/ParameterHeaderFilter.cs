using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthInsuranceSystem.Api.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AcceptsParameterKeyAttribute : Attribute
    {
        public bool IsRequired { get; }

        public AcceptsParameterKeyAttribute(bool isRequired = false)
        {
            IsRequired = isRequired;
        }
    }

    public class ParameterHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            var attr = context.MethodInfo?.GetCustomAttributes(typeof(AcceptsParameterKeyAttribute), true);
            if (attr == null || attr.Length == 0)
                attr = context.MethodInfo?.DeclaringType?.GetCustomAttributes(typeof(AcceptsParameterKeyAttribute), true);

            if (attr != null && attr.Length > 0)
            {
                var isRequired = ((AcceptsParameterKeyAttribute)attr[0]).IsRequired;

                if (isRequired)
                    operation.Security = new List<OpenApiSecurityRequirement>();
            }
        }
    }
}
