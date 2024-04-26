using AutoMapper;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.Domain.Authorization;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services.IService;

using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography;


namespace HealthInsuranceSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, DataContext context, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _passwordService = passwordService;
        }
        public async Task<Result<ResponseModel<List<GetUserDto>>>> GetAllUser()
        {
            var response = new ResponseModel<List<GetUserDto>>();
            var users = await _unitOfWork.UserRepository.GetAll();
            var result = _mapper.Map<List<GetUserDto>>(users);
            response.Data = result;
            return response;
        }

        public async Task<Result<ResponseModel<GetUserDto>>> GetUserByPolicyNumber(string policyNumber)
        {
            var response = new ResponseModel<GetUserDto>();
            var user = await _unitOfWork.UserRepository.GetFirstOrDefault(query => query.UserPolicyNumber == policyNumber);

            if (user == null)
            {
                response.IsSuccessful = false;
                response.Message = UserConstants.ErrorMessages.UserNotFoundWithID;
            }
            else
            {
                try
                {
                    var result = _mapper.Map<GetUserDto>(user);
                    response.IsSuccessful = true;
                    response.Message = UserConstants.Messages.UserValid;
                    response.Data = result;
                }
                catch (Exception ex)
                {
                    return Result.Failure<ResponseModel<GetUserDto>>($"An error has occured - {ex.Message} : {ex.InnerException}");
                }
            }
            return response;
        }

        public async Task<Result<ResponseModel>> CreateUser(AddUserDto request)
        {
            var response = new ResponseModel();
            var user = _mapper.Map<User>(request);
            var role = await _context.Set<Role>()
               .AsNoTracking()
               .Where(x => x.Name == request.RoleType.ToString())
               .FirstOrDefaultAsync();


            user.RoleId = role.RoleId;

            user.UserPolicyNumber = GeneratePolicyHolderId();

            user.CreatedDate = DateTime.Now;
            user.LastModifiedDate = DateTime.Now;
            user.Salt = _passwordService.CreateSalt();
            user.HashPassword = _passwordService.CreateHash(request.Password, user.Salt);
            await _unitOfWork.UserRepository.Add(user);
            try
            {
                await _unitOfWork.SaveAsync();
                response.IsSuccessful = true;
                response.Message = UserConstants.Messages.UserSavedSuccessful;
            }
            catch (Exception ex)
            {
                return Result.Failure<ResponseModel>($"An error has occured - {ex.Message} : {ex.InnerException}");
            }
            return response;
        }

        internal static string GeneratePolicyHolderId()
        {
            const int kLength = 8;
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] tokenBuffer = new byte[kLength];
                rng.GetBytes(tokenBuffer);
                return Convert.ToBase64String(tokenBuffer);
            }
        }
    }
}
