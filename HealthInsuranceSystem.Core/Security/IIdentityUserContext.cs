using AutoMapper;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace HealthInsuranceSystem.Core.Security
{

    public interface IIdentityUserContext
    {
        IdentityUser RequestingUser { get; }
        IdentityUser RequestingCurrentUser { get; }
    }

    public class HttpIdentityUserContext : IIdentityUserContext
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfigurationProvider _automapperConfiguration;
        private IdentityUser _cachedUser;

        public HttpIdentityUserContext(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IConfigurationProvider automapperConfiguration)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _automapperConfiguration = automapperConfiguration;
        }

        public IdentityUser RequestingUser
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userId == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                var user = _dataContext
                    .Set<User>()
                    .Include(x => x.Role)
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Id.ToString() == userId.Value && x.IsActive);

                if (user == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                var currentUser = user.MapTo<IdentityUser>(_automapperConfiguration);

                currentUser.Claims = _dataContext.RoleClaims
                    .Where(x => x.RoleId == user.RoleId)
                    .Select(x => new ClaimDto
                    {
                        ClaimId = x.ClaimId,
                        Description = x.Claim.Description,
                        Name = x.Claim.Name
                    })
                    .ToList();
                currentUser.RoleName = user.Role.Name;

                //currentUser.ByPassAudit = currentUser.Claims
                //    .Any(x => x.Name.ToLower().Contains("bypass read audit"));

                if (currentUser == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                return _cachedUser = currentUser;
            }
        }

        public IdentityUser RequestingCurrentUser
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userId == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                var user = _dataContext
                    .Set<User>()
                    .Include(x => x.Role)
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Id.ToString() == userId.Value && x.IsActive);

                if (user == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                var currentUser = user.MapTo<IdentityUser>(_automapperConfiguration);

                currentUser.Claims = _dataContext.RoleClaims
                    .Where(x => x.RoleId == user.RoleId)
                    .Select(x => new ClaimDto
                    {
                        ClaimId = x.ClaimId,
                        Description = x.Claim.Description,
                        Name = x.Claim.Name
                    })
                    .ToList();
                currentUser.RoleName = user.Role.Name;

                // var moneyInQR = _dataContext
                //.Set<QRCode>()
                //.AsNoTracking()
                //.Where(x => x.UserId == user.Id && (x.ReceiverId == null || x.ReceiverId == 0)).Sum(x => x.Feels.Value);

                //if (moneyInQR != currentUser.MoneyInQR)
                //{
                //    currentUser.MoneyInQR = moneyInQR;
                //    currentUser.UpdateBalance = true;
                //}

                //currentUser.ByPassAudit = currentUser.Claims
                //    .Any(x => x.Name.ToLower().Contains("bypass read audit"));

                if (currentUser == null)
                    throw new AuthorizationFailedException(Constants.Auth.NoUser);

                return _cachedUser = currentUser;
            }
        }
    }

    public class DeferredHttpIdentityUserContext : IIdentityUserContext
    {
        private readonly Lazy<HttpIdentityUserContext> _deferredContext;

        public DeferredHttpIdentityUserContext(Lazy<HttpIdentityUserContext> deferredContext)
        {
            _deferredContext = deferredContext;
        }

        public IdentityUser RequestingUser => _deferredContext.Value.RequestingUser;
        public IdentityUser RequestingCurrentUser => _deferredContext.Value.RequestingCurrentUser;
    }

    public class NullIdentityUserContext : IIdentityUserContext
    {
        public IdentityUser RequestingUser => new IdentityUser
        {
            Id = 0
        };

        public IdentityUser RequestingCurrentUser => new IdentityUser
        {
            Id = 0
        };
    }
}
