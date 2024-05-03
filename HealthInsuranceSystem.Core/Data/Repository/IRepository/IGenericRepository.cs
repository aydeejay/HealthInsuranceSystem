using System.Linq.Expressions;

namespace HealthInsuranceSystem.Core.Data.Repository.IRepository
{
    public interface IGenericRepository<T>
        where T : class
    {
        Task<T> Get(int id);

        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>,
            IOrderedQueryable<T>> orderBy = null, string includeProperties = null);

        Task<T> GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null);

        IQueryable<T> GetQueryable(Expression<Func<T, bool>> filter = null);

        Task<T> Add(T entity);

        void Update(T entity);

        void Remove(int id);

        void Remove(T entity);
    }
}
