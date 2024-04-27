using CSharpFunctionalExtensions;

using FluentValidation;

using HealthInsuranceSystem.Api.Security.Authorization;
using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services.IService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthInsuranceSystem.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : BaseApiController
    {
        private readonly IClaimService _claimService;
        private readonly IValidator<AddClaimDto> _addClaimDtoMappingConfig;
        private readonly IValidator<ReviewClaimDto> _updateClaimDtoMappingConfig;

        public ClaimController(IClaimService claimService,
            IValidator<ReviewClaimDto> updateClaimDtoMappingConfig,
            IValidator<AddClaimDto> addClaimDtoMappingConfig)
        {
            _claimService = claimService;
            _addClaimDtoMappingConfig = addClaimDtoMappingConfig;
            _updateClaimDtoMappingConfig = updateClaimDtoMappingConfig;
        }

        [RequiresClaims(Claims.CanViewAllClaims)]
        [HttpGet("get-all-claims")]
        [Produces(typeof(Envelope<PagedQueryResult<GetClaimDto>>))]
        public async Task<IActionResult> GetAllClaim(
            [FromQuery] PaginatedQuery query)
        {
            var response = await _claimService.GetAllClaims(query);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }

        [RequiresClaims(Claims.CanEditClaims)]
        [HttpPost("add-claim")]
        public async Task<IActionResult> CreateClaim([FromBody] AddClaimDto request)
        {
            var validateModel = await _addClaimDtoMappingConfig.ValidateAsync(request);
            if (!validateModel.IsValid)
            {
                return Error(validateModel.ToString());
            }
            var response = await _claimService.AddClaim(request);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }

        [RequiresClaims(Claims.CanReview)]
        [HttpPut("review-claim")]
        public async Task<IActionResult> ReviewClaim([FromBody] ReviewClaimDto request)
        {
            var validateModel = await _updateClaimDtoMappingConfig.ValidateAsync(request);
            if (!validateModel.IsValid)
            {
                return Error(validateModel.ToString());
            }
            var response = await _claimService.ReviewClaim(request);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }
    }
}
