using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        Task<T> Add(T entity);

        void Update(T entity);

        void Remove(int id);

        void Remove(T entity);
    }
}
