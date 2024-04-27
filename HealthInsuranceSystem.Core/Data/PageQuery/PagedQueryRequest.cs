using HealthInsuranceSystem.Core.Extensions.Constants;

namespace HealthInsuranceSystem.Core.Data.PageQuery
{
    /// <summary>
    /// Request object for a paginated list of results.
    /// </summary>
    public class PagedQueryRequest
    {
        /// <summary>
        /// The page number for the paginated results.  Default is 1.
        /// Setting a number beyond the last page will just return the last page of data.
        /// </summary>
        public int PageNumber { get; set; } = LogicConstants.DefaultPageNumber;

        private int _pageSize = LogicConstants.DefaultPageSize;

        /// <summary>
        /// The number of items returned for the page. A maximum size of 100 is set.  Default is 20.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = Math.Min(value, Constants.MaxPageSize); }
        }
    }
}
