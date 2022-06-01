
namespace Repository
{
    public class RepositoryBase<T, TKey> : IRepositoryBase<T, TKey> where T : class, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> Entities;

        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
            Entities = context.Set<T>();
        }

        public void Create(List<T> entities)
        {
            try
            {
                Entities.AddRange(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DalException($"Insert object failed ", ex);
            }
        }

        public async Task CreateAsync(List<T> entities)
        {
            try
            {
                Entities.AddRange(entities);
               await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Insert object failed ", ex);
            }
        }


        protected int SaveChanges()
        {
           return _context.SaveChanges();
        }

        protected async Task<int> SaveChangesAsync()
        {
           return await _context.SaveChangesAsync();
        }

        int IRepositoryBase<T, TKey>.Delete(TKey id)
        {
            try
            {
                var entity = Entities.Find(id);
                if (entity == null) throw new DalException("Object Not Found Exception");
                Entities.Remove(entity);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed - ID {id} ", ex);
            }
        }

         async Task<int> IRepositoryBase<T, TKey>.DeleteAsync(TKey id)
        {
            try
            {
                var entity = Entities.Find(id);
                if (entity == null) throw new DalException("Object Not Found Exception");
                Entities.Remove(entity);
               return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed - ID {id} ", ex);
            }
        }

        int IRepositoryBase<T, TKey>.Delete(T entity)
        {
            try
            {
                if (entity == null) throw new DalException("Object Not Found Exception");
                Entities.Remove(entity);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed - ID {entity} ", ex);
            }
        }

        async Task<int> IRepositoryBase<T, TKey>.DeleteAsync(T entity)
        {
            try
            {
                if (entity == null) throw new DalException("Object Not Found Exception");
                Entities.Remove(entity);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed - ID {entity} ", ex);
            }
        }

        IEnumerable<T> IRepositoryBase<T, TKey>.GetAll()
        {
            try
            {
                return Entities.AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot retrieve the list of entities ", ex);
            }
        }

        async Task<IEnumerable<T>> IRepositoryBase<T, TKey>.GetAllAsync()
        {
            try
            {
                return await Entities.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot retrieve the list of entities ", ex);
            }
        }


        T IRepositoryBase<T, TKey>.GetByFilter(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                return Entities.SingleOrDefault(filter);
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot retrieve the filtered entity ", ex);
            }
        }

        T IRepositoryBase<T, TKey>.GetById(TKey id)
        {
            try
            {
                return Entities.Find(id);
            }
            catch (Exception ex)
            {
                throw new DalException($"Cannot retrieve the entity - ID {id} ", ex);
            }
        }

        async Task<T> IRepositoryBase<T, TKey>.GetByIdAsync(TKey id)
        {
            try
            {
                return await Entities.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new DalException($"Cannot retrieve the entity - ID {id} ", ex);
            }
        }

        int IRepositoryBase<T, TKey>.GetCount()
        {
            try
            {
                return Entities.Count();
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot get the count of the entities ", ex);
            }
        }

        async Task<int> IRepositoryBase<T, TKey>.GetCountAsync()
        {
            try
            {
                return await Entities.CountAsync();
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot get the count of the entities ", ex);
            }
        }

        int IRepositoryBase<T, TKey>.GetCount(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                return Entities.Count(filter);
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot get the count of the filtered entities ", ex);
            }
        }

        async Task<int> IRepositoryBase<T, TKey>.GetCountAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                return await Entities.CountAsync(filter);
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot get the count of the filtered entities ", ex);
            }
        }

        IQueryable<T> IRepositoryBase<T, TKey>.GetListByFilter(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                IQueryable<T> data = this.Entities.AsQueryable();
                return data.Where(filter);
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot retrieve the filtered list of entities ", ex);
            }
        }
        async Task<IQueryable<T>> IRepositoryBase<T, TKey>.GetListByFilterAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                return await Task.Run(() => this.Entities.AsQueryable().Where(filter)); 
            }
            catch (Exception ex)
            {
                throw new DalException("Cannot retrieve the filtered list of entities ", ex);
            }
        }

        void IRepositoryBase<T, TKey>.Create(T entity)
        {
            if (entity == null)
            {
                throw new DalException($"Object {nameof(entity)} is null ");
            }

            try
            {
                Entities.Add(entity);
                SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DalException($"Insert object {nameof(entity)} failed ", ex);
            }
        }

        async Task<T> IRepositoryBase<T, TKey>.CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new DalException($"Object {nameof(entity)} is null ");
            }

            try
            {
                Entities.Add(entity);
                await SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new DalException($"Insert object {nameof(entity)} failed ", ex);
            }

        }

        int IRepositoryBase<T, TKey>.Update(T entity)
        {
            if (entity == null)
            {
                throw new DalException($"Object {nameof(entity)} is null ");
            }

            try
            {
                _context.Entry(entity).State = EntityState.Modified;
               
               return SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DalException($"Update object {nameof(entity)} failed ", ex);
            }
        }

        async Task<int> IRepositoryBase<T, TKey>.UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new DalException($"Object {nameof(entity)} is null ");
            }

            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Update object {nameof(entity)} failed ", ex);
            }
        }

        public async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new DalException($"Object {nameof(entities)} is null ");
            }

            try
            {
               
                await Entities.AddRangeAsync(entities);
                await SaveChangesAsync();
                return Entities.ToList();
            }
            catch (Exception ex)
            {
                throw new DalException($"Insert object {nameof(entities)} failed ", ex);
            }
        }


        async Task<int> IRepositoryBase<T, TKey>.DeleteRangeAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            try
            {
                var entities = Entities.Where(filter);
                if (entities == null) throw new DalException("Object Not Found Exception");
                Entities.RemoveRange(entities);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed ", ex);
            }
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new DalException($"Object {nameof(entities)} is null ");
            }

            try
            {
                Entities.UpdateRange(entities);
                await SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new DalException($"Update object {nameof(entities)} failed ", ex);
            }
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                
                if (entities == null) throw new DalException("Object Not Found Exception");
                Entities.RemoveRange(entities);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DalException($"Delete entity failed ", ex);
            }
        }
    }
}
