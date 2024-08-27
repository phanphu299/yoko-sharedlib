using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.Repository.Model.Generic;
using AHI.Infrastructure.Service.Enum;

namespace AHI.Infrastructure.Service.Abstraction
{
    public interface ISearchService<TEntity, TKey, TCriteria, TResponse> where TCriteria : BaseCriteria
                                                                            where TEntity : class, IEntity<TKey>
                                                                            where TResponse : class, new()
    {
        Task<BaseSearchResponse<TResponse>> SearchAsync(TCriteria criteria);
        Task<BaseSearchResponse<TResponse>> HierarchySearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID);
        Task<BaseSearchResponse<TResponse>> SearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID);
        Task<BaseSearchResponse<TResponse>> RelationSearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID);
    }
}