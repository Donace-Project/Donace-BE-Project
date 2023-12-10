using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Repository.Base
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> _dbSet;
        private readonly IUserProvider _userProvider;

        public RepositoryBase(CalendarDbContext db, IUserProvider userProvider)
        {
            _dbSet = db.Set<TEntity>();
            _userProvider = userProvider;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            entity.CreatorId = _userProvider.GetUserId();
            entity.CreationTime = DateTime.Now;
            var entityEntry = await _dbSet.AddAsync(entity);

            return entityEntry.Entity;
        }

        public virtual async Task CreateRangeAsync(List<TEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].CreatorId = _userProvider.GetUserId();
                entities[i].CreationTime = DateTime.Now;
            }

            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            entity.LastModificationTime = DateTime.Now;
            entity.LastModifierId = _userProvider.GetUserId();
            _dbSet.Attach(entity);
        }

        public virtual void UpdateRange(List<TEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].LastModificationTime = DateTime.Now;
            }

            _dbSet.AttachRange(entities);
        }

        public virtual void Delete(TEntity entity, bool softDelete = true)
        {
            if (softDelete)
            {
                entity.IsDeleted = true;
                entity.LastModificationTime = DateTime.Now;
                return;
            }

            _dbSet.Remove(entity);
        }

        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).CountAsync();
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
