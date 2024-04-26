using CSharpFunctionalExtensions;

using FluentValidation;

using HealthInsuranceSystem.Api.Security.Authorization;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services.IService;

using Microsoft.AspNetCore.Mvc;

namespace HealthInsuranceSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : BaseApiController
    {
        private readonly IClaimService _claimService;
        private readonly IValidator<AddClaimDto> _addClaimDtoMappingConfig;
        private readonly IValidator<UpdateClaimDto> _updateClaimDtoMappingConfig;

        public ClaimsController(IClaimService claimService,
            IValidator<UpdateClaimDto> updateClaimDtoMappingConfig,
            IValidator<AddClaimDto> addClaimDtoMappingConfig)
        {
            _claimService = claimService;
            _addClaimDtoMappingConfig = addClaimDtoMappingConfig;
            _updateClaimDtoMappingConfig = updateClaimDtoMappingConfig;
        }

        [RequiresClaims(Claims.AcceptRequest)]
        [HttpPost("addClaims")]
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
        [HttpPut("updateClaims")]
        public async Task<IActionResult> UpdateClaim([FromBody] UpdateClaimDto request)
        {
            var validateModel = await _updateClaimDtoMappingConfig.ValidateAsync(request);
            if (!validateModel.IsValid)
            {
                return Error(validateModel.ToString());
            }
            var response = await _claimService.UpdateClaim(request);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }
    }
}
