using FluentValidation;

namespace HealthInsuranceSystem.Api.Configurations
{    
    public static class FluentValidationConfig
    {
        public static void Configure()
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        }
    }
}
