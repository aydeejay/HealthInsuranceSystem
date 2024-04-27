using AutoMapper;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Core.Security
{
    public interface ILoginService
    {
        Task<Result<LoginDto>> GetLoginByNationalId(string nationalId, string password);

        Task<LoginDto> GetLoginById(int id);
    }

    public class LoginDto
    {
        public int Id { get; set; }

        public string NationalId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }
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

        public async Task<Result<LoginDto>> GetLoginByNationalId(string nationalId, string password)
        {

            var user = await _context
                                  .Set<User>()
                                  .Where(x => x.NationalID.ToLower() == nationalId.ToLower())
                                  .FirstOrDefaultAsync()
                                  .ConfigureAwait(false);

            if (user == null)
                return Result.Failure<LoginDto>($"National Id is not associated with a user.");

            var derivedpassword = _passwordService.CreateHash($"{AccessConstants.Messages.AuthGlobalKey}{user.NationalID}", user.Salt).Replace("+", "").Replace(" ", "");

            if (password != derivedpassword)
            {

                if (user.HashPassword != _passwordService.CreateHash(password, user.Salt))
                {
                    return Result.Failure<LoginDto>($"No user found with this NationalId and password combination.");
                }

            }
            if (!user.IsActive)
                return Result.Failure<LoginDto>($"Your account has been deactivated. Contact support for further enquiries");

            return user.MapTo<LoginDto>(_automapperConfiguration);
        }

        public async Task<LoginDto> GetLoginById(int id)
        {
            var user = await _context.Users
               .SingleOrDefaultAsync(x => x.Id == id);

            var loginDto = user.MapTo<LoginDto>(_automapperConfiguration);

            return loginDto;
        }

        public class LoginServiceProfile : Profile
        {
            public LoginServiceProfile()
            {
                CreateMap<User, LoginDto>().ForMember(x => x.Username, options => options
                    .MapFrom(s => $"{s.FirstName}.{s.LastName}"));
            }
        }
    }
}