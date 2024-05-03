using HealthInsuranceSystem.Core.Data.Repository.IRepository;

namespace HealthInsuranceSystem.Core.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IClaimsAuditRepository _claimsAuditRepository;
        private IClaimRepository _claimRepository;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        public UnitOfWork(DataContext context, IClaimsAuditRepository claimAuditRepository, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _context = context;
            _claimsAuditRepository = claimAuditRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        public IClaimsAuditRepository ClaimsAuditRepository => _claimsAuditRepository ??= new ClaimsAuditRepository(_context);
        public IClaimRepository ClaimRepository => _claimRepository ??= new ClaimRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
