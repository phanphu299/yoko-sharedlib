using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Model.Generic;

namespace AHI.Infrastructure.Repository.Generic
{
    public abstract class GenericRepository<TModel, TKey> : IRepository<TModel, TKey>
                                                                    where TModel : class, IEntity<TKey>
                                                                    where TKey : IEquatable<TKey>
    {
        protected Microsoft.EntityFrameworkCore.DbContext Context { get; private set; }

        public GenericRepository(Microsoft.EntityFrameworkCore.DbContext context)
        {
            Context = context;
        }

        public virtual async Task<TModel> AddAsync(TModel e)
        {
            var Collection = Context.Set<TModel>();
            await Collection.AddAsync(e);
            return e;
        }
        public virtual async Task<TModel> UpdateAsync(TKey key, TModel e)
        {
            var trackingEntity = await FindAsync(key);
            if (trackingEntity != null)
            {
                Update(e, trackingEntity);
            }
            return e;

        }
        public virtual Task<TModel> FindAsync(TKey id)
        {
            return AsQueryable().FirstOrDefaultAsync(e => e.Id.Equals(id));
        }
        public virtual async Task<bool> RemoveAsync(TKey key)
        {
            var entity = await FindAsync(key);
            if (entity != null)
            {
                var Collection = Context.Set<TModel>();
                Collection.Remove(entity);
                return true;
            }
            return false;
        }

        public virtual IQueryable<TModel> AsQueryable()
        {
            var Collection = Context.Set<TModel>();
            return Collection;
        }
        protected abstract void Update(TModel requestObject, TModel targetObject);

        public virtual IQueryable<TModel> AsFetchable()
        {
            return AsQueryable();
        }
    }
}
