using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Model.Generic;

namespace AHI.Infrastructure.Repository.Generic
{
    public interface IRepository<TModel, TKey> : ISearchRepository<TModel, TKey>, IFetchRepository<TModel, TKey> where TModel : class, IEntity<TKey>
    {
        Task<TModel> AddAsync(TModel e);
        Task<TModel> UpdateAsync(TKey key, TModel e);
        Task<bool> RemoveAsync(TKey key);
        Task<TModel> FindAsync(TKey id);
    }
}
