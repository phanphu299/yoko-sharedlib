using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using AHI.Infrastructure.Service.Tag.Model;

namespace AHI.Infrastructure.Service.Tag.Service.Abstraction
{
    public interface IEntityTagRepository<TEntity> : IRepository<TEntity, long> where TEntity : EntityTag
    {
        Task AddWithSaveChangeAsync(TEntity entity);
        Task AddRangeWithSaveChangeAsync(TEntity[] entities);
        Task AddRangeAsync(TEntity[] entities);
        void RemoveRange(TEntity[] entities);
        Task RemoveByEntityIdsAsync<T>(string entityType, IList<T> entityIds, bool isTracking = false);
        Task RemoveByEntityIdAsync<T>(string entityType, T entityId, bool isTracking = false);
    }
}