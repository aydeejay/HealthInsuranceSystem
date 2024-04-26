using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Models.Domain;

namespace HealthInsuranceSystem.Core.Data.Repository
{
    public class ClaimRepository : GenericRepository<Claim>, IClaimRepository
    {
        private readonly DataContext context;

        public ClaimRepository(DataContext context) : base(context)
        {
            this.context = context;
        }
    }
}
