using System.Linq.Expressions;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class 
    {
        T Find(int id);

        ICollection<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true
            );

        T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true
            );

        void Add(T entity);
        
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

        void Save();
    }
}
