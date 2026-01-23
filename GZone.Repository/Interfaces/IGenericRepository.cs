using System.Linq.Expressions;

namespace GZone.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        bool HasChanges(T newEntity, T trackedEntity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);

        Task<List<TResult>?> GetListAsync<TResult>(
            Expression<Func<T, bool>> whereLinQ,
            Expression<Func<T, TResult>> selectLinQ,
            bool hasTrackings = true);
        Task<List<T>?> GetListAsync(
            Expression<Func<T, bool>> whereLinQ,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool useSplitQuery = true,
            bool hasTrackings = true);

        Task<T?> GetOneAsync(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool hasTrackings = true);
        Task<T?> GetByIdAsync<Tkey>(Tkey id);

        System.Threading.Tasks.Task AddAsync(T TEntity);
        Task AddRangeAsync(IEnumerable<T> Tentities);

        Task UpdateAsync(T TEntity);

        Task DeleteAsync(T TEntity);
        Task DeleteRangeAsync(IEnumerable<T> TEntities);

        Task<bool> SaveChangeAsync();
    }

}
