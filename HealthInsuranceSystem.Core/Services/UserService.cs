using AutoMapper;
using AutoMapper.QueryableExtensions;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Data.PageQuery;
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
        private readonly IConfigurationProvider _configurationProvider;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, DataContext context, IPasswordService passwordService, IConfigurationProvider configurationProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _passwordService = passwordService;
            _configurationProvider = configurationProvider;
        }
        public async Task<Result<PagedQueryResult<GetUserDto>>> GetAllUser(PaginatedQuery query)
        {
            var result = new PagedQueryResult<GetUserDto>();

            var user = await _unitOfWork.UserRepository.GetAll();
            var users = _context.Set<User>()
            .AsNoTracking()
            .Include(x => x.Role)
            .AsQueryable();

            var search = query.Search?.ToLower();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.NationalID.Contains(search) ||
                    x.UserPolicyNumber.Contains(search));
            }

            if (query.PolicyHolderId != null && query.PolicyHolderId != 0)
            {
                users = users.Where(x => x.Id == query.PolicyHolderId);
            }

            result.Items = await users
           .ProjectTo<GetUserDto>(_configurationProvider)
           .Skip((query.PageQuery.PageNumber - 1) * query.PageQuery.PageSize)
           .Take(query.PageQuery.PageSize)
           .ToListAsync();

            result.TotalItemCount = await users.CountAsync();
            result.PageCount = (result.TotalItemCount + query.PageQuery.PageSize - 1) / query.PageQuery.PageSize;
            result.PageNumber = query.PageQuery.PageNumber;
            result.PageSize = query.PageQuery.PageSize;

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

            var user = await _context.Set<User>()
               .AsNoTracking()
               .Where(x => x.NationalID.ToLower() == request.NationalID.ToLower())
               .FirstOrDefaultAsync();

            if (user != null)
            {
                return Result.Failure<ResponseModel>(UserConstants.ErrorMessages.UserExists);
            }

            user = _mapper.Map<User>(request);
            var role = await _context.Set<Role>()
               .AsNoTracking()
               .Where(x => x.Name == request.RoleType.ToString())
               .FirstOrDefaultAsync();


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
    }
}
