using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Donace_BE_Project.EntityFramework.Repository.Base
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> _dbSet;

        public RepositoryBase(CalendarDbContext db)
        public const string IdModifier = "623e0c1a-93f0-4b1f-a85a-47f632b3ad77";
        public RepositoryBase(AppDbContext db)
        {
            _dbSet = db.Set<TEntity>();
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
            entity.CreatorId = Guid.Parse(IdModifier);
            entity.CreationTime = DateTime.Now;
            var entityEntry = await _dbSet.AddAsync(entity);

            return entityEntry.Entity;
        }

        public virtual async Task CreateRangeAsync(List<TEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].CreatorId = Guid.Parse(IdModifier);
                entities[i].CreationTime = DateTime.Now;
            }

            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            entity.LastModificationTime = DateTime.Now;
            entity.LastModifierId = Guid.Parse(IdModifier);
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(List<TEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].LastModificationTime = DateTime.Now;
            }

            _dbSet.UpdateRange(entities);
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

        public virtual async Task<long> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await Task.FromResult(_dbSet.AsQueryable());
        }
    }
}
