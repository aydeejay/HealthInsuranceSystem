namespace HealthInsuranceSystem.Core.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IClaimsAuditRepository ClaimsAuditRepository { get; }
        IClaimRepository ClaimRepository { get; }
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        Task SaveAsync();
    }
}
