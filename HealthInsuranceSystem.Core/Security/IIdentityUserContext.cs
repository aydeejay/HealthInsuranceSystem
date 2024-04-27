using AutoMapper;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Exceptions;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.RoleClaimsDto;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace HealthInsuranceSystem.Core.Security
{

    public interface IIdentityUserContext
    {
        IdentityUser RequestingUser { get; }
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

                currentUser.Claims = _dataContext.RoleAuthClaims
                    .Where(x => x.RoleId == user.RoleId)
                    .Select(x => new ClaimDto
                    {
                        ClaimId = x.AuthClaimId,
                        Description = x.AuthClaim.Description,
                        Name = x.AuthClaim.Name
                    })
                    .ToList();
                currentUser.RoleName = user.Role.Name;

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
