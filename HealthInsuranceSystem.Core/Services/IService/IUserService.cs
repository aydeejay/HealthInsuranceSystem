using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;

namespace HealthInsuranceSystem.Core.Services.IService
{
    public interface IUserService
    {
        Task<Result<ResponseModel>> CreateUser(AddUserDto request);
        Task<Result<ResponseModel<GetUserDto>>> GetUserByPolicyNumber(string policyNumber);
        Task<Result<ResponseModel<List<GetUserDto>>>> GetAllUser();
    }
}
