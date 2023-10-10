using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Entities.Calendar;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : BaseEntity
    {
        Task<long> CountAsync();
        Task<TEntity> CreateAsync(TEntity entity);
        Task CreateRangeAsync(List<TEntity> entities);
        void Delete(TEntity entity, bool softDelete = true);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
