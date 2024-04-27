using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;

namespace HealthInsuranceSystem.Core.Services.IService
{
    public interface IClaimService
    {
        Task<Result<ResponseModel>> ReviewClaim(ReviewClaimDto request);
        Task<Result<ResponseModel>> AddClaim(AddClaimDto request);
        Task<Result<ResponseModel<PagedQueryResult<GetClaimDto>>>> GetAllClaims(PaginatedQuery request);
    }
}
