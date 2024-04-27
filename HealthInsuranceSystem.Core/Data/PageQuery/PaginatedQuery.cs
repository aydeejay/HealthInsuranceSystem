namespace HealthInsuranceSystem.Core.Data.PageQuery
{
    public class PaginatedQuery
    {
        public PagedQueryRequest PageQuery { get; set; }

        public string Search { get; set; } = string.Empty;

        public bool SortAscending { get; set; } = false;

        public int? PolicyHolderId { get; set; }
    }
}