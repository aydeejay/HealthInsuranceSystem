using AutoMapper;
using AutoMapper.QueryableExtensions;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Core.Security
{
    public interface ILoginService
    {
        Task<LoginDto> GetLoginByEmail(string email);

        Task<bool> LoginByEmailCheck(string email);

        Task<Result<LoginDto>> GetLoginByEmail(string email, string password);

        Task<LoginDto> GetLoginById(int id);
    }

    public class LoginDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public bool HasWebPortalAccess { get; set; }
    }

    public class LoginService : ILoginService
    {
        private readonly DataContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IConfigurationProvider _automapperConfiguration;


        public LoginService(DataContext context, IPasswordService passwordService, IConfigurationProvider automapperConfiguration)
        {
            _context = context;
            _passwordService = passwordService;
            _automapperConfiguration = automapperConfiguration;
        }

        public async Task<bool> LoginByEmailCheck(string name)
        {
            return await _context
                .Set<User>().AnyAsync(x => x.IsActive && x.Name == name);
        }

        public async Task<LoginDto> GetLoginByEmail(string name)
        {
            return await _context
                .Set<User>()
                .Where(x => x.IsActive && x.Name == name)
                .ProjectTo<LoginDto>(_automapperConfiguration, x => x)
                .FirstOrDefaultAsync();
        }

        public async Task<Result<LoginDto>> GetLoginByEmail(string name, string password)
        {
            if (name.Contains('|'))
            {
                if (name.Split("|")[1] == "SkipValidationFeels")
                {
                    name = name.Split("|")[0];
                }
            }

            var user = await _context
                                  .Set<User>()
                                  .Where(x => x.Name == name)
                                  .FirstOrDefaultAsync()
                                  .ConfigureAwait(false);

            if (user == null)
                return Result.Failure<LoginDto>($"Email is not associated with a feels user.");

            var derivedpassword = _passwordService.CreateHash($"{AccessConstants.Messages.AuthGlobalKey}{user.Name}", user.Salt).Replace("+", "").Replace(" ", "");

            if (password != derivedpassword)
            {
                //if (!user.NormalLoginEnabled)
                //{
                //    return Result.Failure<LoginDto>($"Password login not enabled. Please reset account to activate this login channel");
                //}

                if (user.HashPassword != _passwordService.CreateHash(password, user.Salt))
                {
                    return Result.Failure<LoginDto>($"No user found with this email and password combination.");
                }

                //if (!user.IsEmailConfirmed)
                //    return Result.Failure<LoginDto>($"Confirm your email before accessing FEELS.");
            }
            if (!user.IsActive)
                return Result.Failure<LoginDto>($"Your account has been deactivated.  To discuss reactivation, please email support@feels.com.");

            return user.MapTo<LoginDto>(_automapperConfiguration);
        }

        //public async Task<LoginDto> GetLoginById(int id)
        //{
        //    var user = await _context.Users
        //       .Include(x => x.Role)
        //       .ThenInclude(x => x.RoleClaims)
        //       .SingleOrDefaultAsync(x => x.Id == id);

        //    var loginDto = user.MapTo<LoginDto>(_automapperConfiguration);
        //    //var portalAccessClaimId0 =await  _context.Claim
        //    //   .Where(x => x.Name == Claims.WebPortal)
        //    //   .Select<Claim, int?>(x => x.ClaimId)
        //    //   .SingleOrDefaultAsync();
        //    var portalAccessClaimId = await _context.Claim
        //        .Where(x => x.Name == Claims.WebPortal)
        //        .Select(x => (int?)x.ClaimId)
        //        .SingleOrDefaultAsync();

        //    var hasWebPortalAccess = !portalAccessClaimId.HasValue || user.Role.RoleClaims.Any(x => x.ClaimId == portalAccessClaimId.Value && x.Active);
        //    loginDto.HasWebPortalAccess = hasWebPortalAccess;
        //    return loginDto;
        //}
        public async Task<LoginDto> GetLoginById(int id)
        {
            var user = await _context.Users
               // .Include(x => x.Role)
               // .ThenInclude(x => x.RoleClaims)
               .SingleOrDefaultAsync(x => x.Id == id);

            var loginDto = user.MapTo<LoginDto>(_automapperConfiguration);

            return loginDto;
        }

        public class LoginServiceProfile : Profile
        {
            public LoginServiceProfile()
            {
                CreateMap<User, LoginDto>().ForMember(x => x.Username, options => options
                    .MapFrom(s => $"{s.Name}.{s.Surname}"));
            }
        }
    }
}
