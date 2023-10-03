﻿using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : BaseEntity
    {
        Task<long> CountAsync();
        Task<TEntity> CreateAsync(TEntity entity);
        void DeleteAsync(TEntity entity, bool softDelete = true);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id);
        void Update(TEntity entity);
    }
}