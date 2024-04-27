using AutoMapper;
using AutoMapper.QueryableExtensions;

using HealthInsuranceSystem.Core.Data.PageQuery;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> GetPagedData<T>(
            this IQueryable<T> queryable,
            PagedQueryRequest request)
        {
            return queryable.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        public static async Task<PagedQueryResult<TOut>> ToPagedResult<T, TOut>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            IConfigurationProvider configurationProvider,
            CancellationToken token = default)
        {
            if (pageSize == -1)
            {
                return new PagedQueryResult<TOut>
                {
                    Items = new List<TOut>(),
                    PageCount = 0,
                    PageNumber = 0,
                    PageSize = -1,
                    TotalItemCount = 0
                };
            }

            var totalItemCount = await source.CountAsync(token);
            token.ThrowIfCancellationRequested();
            pageSize = pageSize < 1 ? totalItemCount : pageSize;

            var totalPageCount = totalItemCount == 0 ? 1 : (totalItemCount + pageSize - 1) / pageSize;
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPageCount));

            var startIndex = (pageNumber - 1) * pageSize;

            var items = await source
                .Skip(startIndex)
                .Take(pageSize)
                .ProjectTo<TOut>(configurationProvider)
                .ToListAsync(token);

            return new PagedQueryResult<TOut>
            {
                Items = items,
                PageCount = totalPageCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItemCount = totalItemCount
            };
        }

        public static bool RelatedEntityExists<T>(this IQueryable<T> set, Func<T, bool> expression)
            where T : class
        {
            return set.Any(expression);
        }
    }
}
