using FluentValidation;

using HealthInsuranceSystem.Core.Data.Repository;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Services;
using HealthInsuranceSystem.Core.Services.IService;

namespace HealthInsuranceSystem.Api.Configurations
{
    public static class ServiceExtensionConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddScoped(typeof(GenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClaimsAuditRepository, ClaimsAuditRepository>();
            services.AddScoped<IClaimRepository, ClaimRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClaimService, ClaimService>();

            services.AddScoped<IValidator<AddClaimDto>, AddClaimDtoValidator>();
            services.AddScoped<IValidator<AddUserDto>, AddUserDtoValidator>();
            services.AddScoped<IValidator<ReviewClaimDto>, ReviewClaimDtoValidator>();
        }
    }
}
