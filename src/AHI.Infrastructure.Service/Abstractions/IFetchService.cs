using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Model.Generic;

namespace AHI.Infrastructure.Service.Abstraction
{
    public interface IFetchService<TEntity, TKey, TResponse> where TEntity : class, IEntity<TKey>
                                                                            where TResponse : class, new()
    {

        public Task<TResponse> FetchAsync(TKey id);
    }
}