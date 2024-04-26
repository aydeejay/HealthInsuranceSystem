using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Models.Domain;

namespace HealthInsuranceSystem.Core.Data.Repository
{
    public class ClaimsAuditRepository : GenericRepository<ClaimsAudit>, IClaimsAuditRepository
    {
        private readonly DataContext context;

        public ClaimsAuditRepository(DataContext context) : base(context)
        {
            this.context = context;
        }
    }
}
