using HealthInsuranceSystem.Core.Data.PageQuery;

namespace HealthInsuranceSystem.Core.Models.DTO.ClaimsDto
{
    public class GetAllClaimsRequest
    {
        public PagedQueryRequest Query { get; set; }

        public string Search { get; set; }

        public string SortBy { get; set; }

        public bool SortAscending { get; set; }

        public int? PolicyHolderId { get; set; }
    }
}
