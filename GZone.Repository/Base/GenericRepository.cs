using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GZone.Repository.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GZoneDbContext _context;
        public GenericRepository(GZoneDbContext context)
        {
            _context = context;
        }

        //=============================================
        public bool HasChanges(T newEntity, T trackedEntity)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var val1 = prop.GetValue(trackedEntity);
                var val2 = prop.GetValue(newEntity);

                //If not equal => true
                if (!object.Equals(val1, val2)) return true;
            }

            return false;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AnyAsync(expression);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().CountAsync(expression);
        }

        //public async Task<List<T>?> GetListAsync(
        //    Expression<Func<T, bool>> expression,
        //    Func<IQueryable<T>, IQueryable<T>>? include = null,
        //    bool hasTrackings = true,
        //    bool useSplitQuery = true
        //)
        //{
        //    IQueryable<T> query = _context.Set<T>().Where(expression);

        //    if (include != null)
        //        query = include(query);

        //    if (!hasTrackings)
        //        query = query.AsNoTracking();

        //    if (useSplitQuery)
        //        query = query.AsSplitQuery();

        //    return await query.ToListAsync();
        //}

        public async Task<List<TResult>?> GetListAsync<TResult>(
            Expression<Func<T, bool>> whereLinQ,
            Expression<Func<T, TResult>> selectLinQ,
            bool hasTrackings = true)
        {
            return hasTrackings ? await _context.Set<T>().Where(whereLinQ).Select(selectLinQ).ToListAsync()
                                : await _context.Set<T>().Where(whereLinQ).AsNoTracking().Select(selectLinQ).ToListAsync();
        }
        public async Task<List<T>?> GetListAsync(
            Expression<Func<T, bool>> whereLinQ,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool useSplitQuery = true,
            bool hasTrackings = true)
        {
            IQueryable<T> query = _context.Set<T>().Where(whereLinQ);

            if (include is not null) query = include(query);
            if (!hasTrackings) query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (include != null)
            {
                query = include(query);
            }
            if (pageNumber == 0) pageNumber = 1;
            if (pageSize == 0) pageSize = 10;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .AsNoTracking()
                              .ToListAsync();
        }

        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool hasTracking = true)
        {
            IQueryable<T> query = _context.Set<T>().Where(expression);

            if (include is not null) query = include(query);
            if (!hasTracking) query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync<Tkey>(Tkey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public Task AddAsync(T TEntity)
        {
            _context.Add(TEntity);
            return Task.CompletedTask;
        }

        public async Task AddRangeAsync(IEnumerable<T> Tentities)
        {
            await _context.Set<T>().AddRangeAsync(Tentities);
        }

        public Task UpdateAsync(T TEntity)
        {
            _context.Set<T>().Update(TEntity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T TEntity)
        {
            _context.Remove(TEntity);
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync(IEnumerable<T> TEntities)
        {
            _context.Set<T>().RemoveRange(TEntities);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
