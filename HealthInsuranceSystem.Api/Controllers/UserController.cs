﻿using CSharpFunctionalExtensions;

using FluentValidation;

using HealthInsuranceSystem.Api.Security.Authorization;
using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services.IService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthInsuranceSystem.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IValidator<AddUserDto> _addUserDtoMappingConfig;

        public UserController(IUserService userService, IValidator<AddUserDto> addUserDtoMappingConfig)
        {
            _userService = userService;
            _addUserDtoMappingConfig = addUserDtoMappingConfig;
        }


        [RequiresClaims(Claims.CanViewAllUsers)]
        [HttpGet("getAllUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Envelope<ResponseModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Envelope))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUser([FromQuery] PaginatedQuery query)
        {
            var response = await _userService.GetAllUser(query);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }

        [AllowAnonymous]
        [HttpPost("addUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Envelope<ResponseModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Envelope))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] AddUserDto request)
        {
            var validateModel = await _addUserDtoMappingConfig.ValidateAsync(request);
            if (!validateModel.IsValid)
            {
                return Error(validateModel.ToString());
            }
            var response = await _userService.CreateUser(request);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }

        [HttpGet("getUserByPolicyNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Envelope<ResponseModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Envelope))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string policyNumber)
        {

            var response = await _userService.GetUserByPolicyNumber(policyNumber);
            Result res = Result.Combine(response);
            if (res.IsFailure)
                return Error(res.Error);
            return Ok(response.Value);
        }
    }
}
