using AutoMapper;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services.IService;

using System.Security.Cryptography;


namespace HealthInsuranceSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IConfigurationProvider _configurationProvider;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordService passwordService, IConfigurationProvider configurationProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordService = passwordService;
            _configurationProvider = configurationProvider;
        }
        public async Task<Result<ResponseModel<PagedQueryResult<GetUserDto>>>> GetAllUser(PaginatedQuery query)
        {
            var result = new ResponseModel<PagedQueryResult<GetUserDto>>();

            var userQueryable = _unitOfWork.UserRepository.GetQueryable();

            var search = query.Search?.ToLower();

            if (!string.IsNullOrEmpty(search))
            {
                userQueryable = userQueryable.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.NationalID.Contains(search) ||
                    x.UserPolicyNumber.Contains(search));
            }

            if (query.PolicyHolderId != null && query.PolicyHolderId != 0)
            {
                userQueryable = userQueryable.Where(x => x.Id == query.PolicyHolderId);
            }

            result.Data = await userQueryable.
                ToPagedResult<User, GetUserDto>(query.PageQuery.PageNumber, query.PageQuery.PageSize, _configurationProvider);

            return result;
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
            //Check if user is an Admin and the kind of rolerequested for user
            if (request.RoleType != RoleType.PolicyHolder)
            {
                return Result.Failure<ResponseModel>(UserConstants.ErrorMessages.InvalidUserRole);
            }
            var response = new ResponseModel();

            var user = await _unitOfWork.UserRepository.GetFirstOrDefault(x => x.NationalID.ToLower() == request.NationalID.ToLower());

            if (user != null)
            {
                return Result.Failure<ResponseModel>(UserConstants.ErrorMessages.UserExists);
            }

            user = _mapper.Map<User>(request);

            var role = await _unitOfWork.RoleRepository.GetFirstOrDefault(x => x.Name == request.RoleType.ToString());

            user.RoleId = role.RoleId;

            user.UserPolicyNumber = GeneratePolicyHolderId();

            user.IsActive = true;
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

        public static string GeneratePolicyHolderIdTest()
        {
            return GeneratePolicyHolderId();
        }
    }
}
