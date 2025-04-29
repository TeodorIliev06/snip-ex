namespace SnipEx.Data.Repositories
{
    using System.Linq.Expressions;

    using Microsoft.EntityFrameworkCore;

    using Contracts;

    public class BaseRepository<TType, TId> : IRepository<TType, TId>
        where TType : class
    {
        private readonly SnipExDbContext dbContext;
        private readonly DbSet<TType> dbSet;

        public BaseRepository(SnipExDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<TType>();
        }

        public TType? GetById(TId id)
        {
            return this.dbSet.Find(id);
        }

        public async Task<TType?> GetByIdAsync(TId id)
        {
            return await this.dbSet.FindAsync(id);
        }

        public TType? FirstOrDefault(Func<TType, bool> predicate)
        {
            return this.dbSet.FirstOrDefault(predicate);
        }

        public async Task<TType?> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            return await this.dbSet.FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<TType> GetAll()
        {
            return this.dbSet.ToArray();
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await this.dbSet.ToArrayAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return this.dbSet.AsQueryable();
        }

        public void Add(TType item)
        {
            this.dbSet.Add(item);
        }

        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
        }

        public void AddRange(TType[] items)
        {
            this.dbSet.AddRange(items);
        }

        public async Task AddRangeAsync(TType[] items)
        {
            await this.dbSet.AddRangeAsync(items);
        }

        public bool Delete(TId id)
        {
            var entity = this.GetById(id);

            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(TId id)
        {
            var entity = await this.GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(TType entity)
        {
            if (entity == null)
            {
                return false;
            }

            dbSet.Remove(entity);

            return true;
        }

        public bool Update(TType item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.dbContext.Entry(item).State = EntityState.Modified;

                return true;
            }
            catch
            {
                return false;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> UpdateAsync(TType item)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                this.dbSet.Attach(item);
                this.dbContext.Entry(item).State = EntityState.Modified;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await this.dbContext.SaveChangesAsync();
        }
    }
}
