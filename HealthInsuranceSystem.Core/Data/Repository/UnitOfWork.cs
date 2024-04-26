using HealthInsuranceSystem.Core.Data.Repository.IRepository;

namespace HealthInsuranceSystem.Core.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IClaimsAuditRepository _claimsAuditRepository;
        private IClaimRepository _claimRepository;
        private IUserRepository _userRepository;
        public UnitOfWork(DataContext context, IClaimsAuditRepository claimAuditRepository, IUserRepository userRepository)
        {
            _context = context;
            _claimsAuditRepository = claimAuditRepository;
            _userRepository = userRepository;
        }
        public IClaimsAuditRepository ClaimsAuditRepository => _claimsAuditRepository ??= new ClaimsAuditRepository(_context);
        public IClaimRepository ClaimRepository => _claimRepository ??= new ClaimRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
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
