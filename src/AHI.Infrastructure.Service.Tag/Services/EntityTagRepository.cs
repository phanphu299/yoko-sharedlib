using AHI.Infrastructure.Repository.Generic;
using AHI.Infrastructure.Service.Tag.Configuration;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Service.Tag.Service
{
    public class EntityTagRepository<TEntity> : GenericRepository<TEntity, long>, IEntityTagRepository<TEntity> where TEntity : EntityTag
    {
        private readonly DbContext _dbContext;

        public EntityTagRepository(IServiceProvider serviceProvider) : base(serviceProvider.GetService(DbConfig.DbContextType) as DbContext)
        {
            _dbContext = serviceProvider.GetService(DbConfig.DbContextType) as DbContext;
        }

        protected override void Update(TEntity requestObject, TEntity targetObject)
        {
        }

        public Task AddRangeAsync(TEntity[] entities)
        {
            var dbSet = Context.Set<TEntity>();
            return dbSet.AddRangeAsync(entities);
        }

        public void RemoveRange(TEntity[] entities)
        {
            var dbSet = Context.Set<TEntity>();
            dbSet.RemoveRange(entities);
        }

        public async Task RemoveByEntityIdsAsync<T>(string entityType, IList<T> entityIds, bool isTracking = false)
        {
            var dbSet = Context.Set<TEntity>();

            var query = dbSet.AsQueryable();

            if (!isTracking)
                query = query.AsNoTracking();

            query = query.Where(x => x.EntityType == entityType);

            if (entityIds is IList<string> stringEntityIds)
            {
                query = query.Where(x => x.EntityIdString != null && stringEntityIds.Any(a => a == x.EntityIdString));
            }
            else if (entityIds is IList<long> longEntityIds)
            {
                query = query.Where(x => x.EntityIdLong != null && longEntityIds.Select(id => (long?)id).Any(a => a == x.EntityIdLong));
            }
            else if (entityIds is IList<Guid> guidgEntityIds)
            {
                query = query.Where(x => x.EntityIdGuid != null && guidgEntityIds.Select(id => (Guid?)id).Any(a => a == x.EntityIdGuid.Value));
            }
            else if (entityIds is IList<int> intEntityIds)
            {
                query = query.Where(x => x.EntityIdInt != null && intEntityIds.Select(id => (int?)id).Any(a => a == x.EntityIdInt));
            }
            else
                throw new NotSupportedException();

            var entities = await query.ToArrayAsync();
            if (entities.Any())
            {
                dbSet.RemoveRange(entities);
            }
        }

        public async Task RemoveByEntityIdAsync<T>(string entityType, T entityId, bool isTracking = false)
        {
            var dbSet = Context.Set<TEntity>();

            var query = dbSet.AsQueryable();

            if (!isTracking)
                query = query.AsNoTracking();

            query = query.Where(x => x.EntityType == entityType);

            if (entityId is string stringEntityId)
            {
                query = query.Where(x => x.EntityIdString != null && stringEntityId == x.EntityIdString);
            }
            else if (entityId is long longEntityId)
            {
                query = query.Where(x => x.EntityIdLong != null && longEntityId == x.EntityIdLong);
            }
            else if (entityId is Guid guidgEntityId)
            {
                query = query.Where(x => x.EntityIdGuid != null && guidgEntityId == x.EntityIdGuid);
            }
            else if (entityId is int intEntityId)
            {
                query = query.Where(x => x.EntityIdInt != null && intEntityId == x.EntityIdInt);
            }
            else
                throw new NotSupportedException();

            var entities = await query.ToArrayAsync();
            if (entities.Any())
            {
                dbSet.RemoveRange(entities);
            }
        }

        public async Task AddWithSaveChangeAsync(TEntity entity)
        {
            var dbSet = Context.Set<TEntity>();
            await dbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task AddRangeWithSaveChangeAsync(TEntity[] entities)
        {
            var dbSet = Context.Set<TEntity>();
            foreach (var entity in entities)
            {
                await dbSet.AddAsync(entity);
                await Context.SaveChangesAsync();
            }
        }
    }
}