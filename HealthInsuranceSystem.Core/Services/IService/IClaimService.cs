using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;

namespace HealthInsuranceSystem.Core.Services.IService
{
    public interface IClaimService
    {
        Task<Result<ResponseModel>> UpdateClaim(UpdateClaimDto request);
        Task<Result<ResponseModel>> AddClaim(AddClaimDto request);
    }
}
