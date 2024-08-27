using System.Linq;
using AHI.Infrastructure.Repository.Model.Generic;

namespace AHI.Infrastructure.Repository.Generic
{
    public interface IFetchRepository<TModel, TKey> where TModel : class, IEntity<TKey>
    {
        IQueryable<TModel> AsFetchable();
    }
}