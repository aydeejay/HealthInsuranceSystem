namespace HealthInsuranceSystem.Core.Data.PageQuery
{
    public class PagedQueryResult<T>
    {
        private List<T> _items;

        public List<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }

        /// <summary>
        /// The total number of items for this query.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// The number of items to return for each page. This will only return a maximum of 100 items.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        public int PageCount { get; set; }
    }
}
