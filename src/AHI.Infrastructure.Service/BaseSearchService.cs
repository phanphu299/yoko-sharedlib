using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Repository.Generic;
using AHI.Infrastructure.Repository.Model.Generic;
using AHI.Infrastructure.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.SystemContext.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.UserContext.Extension;
using AHI.Infrastructure.UserContext.Service.Abstraction;
using AHI.Infrastructure.Service.Model;
using AHI.Infrastructure.Service.Enum;

namespace AHI.Infrastructure.Service
{
    public abstract class BaseSearchService<TEntity, TKey, TCriteria, TResponse> : ISearchService<TEntity, TKey, TCriteria, TResponse>, IFetchService<TEntity, TKey, TResponse>
                                                                                                where TCriteria : BaseCriteria
                                                                                                where TEntity : class, IEntity<TKey>
                                                                                                where TResponse : class, new()
    {
        private readonly IServiceProvider _serviceProvider;

        protected Func<TEntity, TResponse> convertToModel { get; private set; }

        public BaseSearchService(Func<TEntity, TResponse> convertToModel, IServiceProvider serviceProvider)
        {
            this.convertToModel = convertToModel;
            _serviceProvider = serviceProvider;
        }

        public Task<BaseSearchResponse<TResponse>> RelationSearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID)
        {
            var securityContext = _serviceProvider.GetRequiredService<ISecurityContext>();

            var jArray = new JArray();
            if (IsValidFilter(criteria.Filter))
            {
                var originFilter = JsonConvert.DeserializeObject(criteria.Filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting) as JObject;
                var token = originFilter.Property("and");
                if (token != null)
                {
                    var subArray = (JArray)token.Value;
                    foreach (var item in subArray)
                    {
                        jArray.Add(item);
                    }
                }
                else
                {
                    jArray.Add(originFilter);
                }
            }
            if (!securityContext.FullAccess)
            {
                if (securityContext.RestrictedIds.Any())
                {
                    jArray.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.RestrictedIds), "nin")));
                }
                if (securityContext.AllowedIds.Any())
                {
                    var orFilters = new JArray();
                    orFilters.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.AllowedIds), "in")));
                    orFilters.Add(JObject.FromObject(FilterModel.CreateFrom("createdBy", PageSearchType.TEXT, securityContext.Upn, "eq")));
                    jArray.Add(JObject.FromObject(new
                    {
                        or = orFilters
                    }));
                }
                else
                {
                    // add the owner into query. For DependentEntity created by should be into "and" condition.
                    jArray.Add(JObject.FromObject(FilterModel.CreateFrom("createdBy", PageSearchType.TEXT, securityContext.Upn, "eq")));
                }
            }

            if (jArray.Any())
            {
                // in this case, the filter should be:
                // AND[{originFilter},{not in restricted}, {created_by}, OR[{allowIds}]
                object filter = new
                {
                    and = jArray
                };
                if (jArray.Count == 1)
                {
                    // no need and, only the object filter
                    filter = jArray.First;
                }
                criteria.Filter = JsonConvert.SerializeObject(filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            }
            return SearchAsync(criteria);
        }

        public Task<BaseSearchResponse<TResponse>> SearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID)
        {
            var securityContext = _serviceProvider.GetRequiredService<ISecurityContext>();

            var jArray = new JArray();
            if (IsValidFilter(criteria.Filter))
            {
                var originFilter = JsonConvert.DeserializeObject(criteria.Filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting) as JObject;
                var token = originFilter.Property("and");
                if (token != null)
                {
                    var subArray = (JArray)token.Value;
                    foreach (var item in subArray)
                    {
                        jArray.Add(item);
                    }
                }
                else
                {
                    jArray.Add(originFilter);
                }
            }
            if (!securityContext.FullAccess)
            {
                if (securityContext.RestrictedIds.Any())
                {
                    jArray.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.RestrictedIds), "nin")));
                }
                var orFilters = new JArray();
                // add the owner into query
                orFilters.Add(JObject.FromObject(FilterModel.CreateFrom("createdBy", PageSearchType.TEXT, securityContext.Upn, "eq")));
                if (securityContext.AllowedIds.Any())
                {
                    orFilters.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.AllowedIds), "in")));
                }
                jArray.Add(JObject.FromObject(new
                {
                    or = orFilters
                }));
            }

            if (jArray.Any())
            {
                // in this case, the filter should be:
                // AND[{originFilter},{not in restricted},OR[{created_by, allowIds}]
                object filter = new
                {
                    and = jArray
                };
                if (jArray.Count == 1)
                {
                    // no need and, only the object filter
                    filter = jArray.First;
                }
                criteria.Filter = JsonConvert.SerializeObject(filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            }
            return SearchAsync(criteria);
        }
        public Task<BaseSearchResponse<TResponse>> HierarchySearchWithSecurityAsync(TCriteria criteria, string objectKeyName = "id", PageSearchType objectKeyType = PageSearchType.GUID)
        {
            var securityContext = _serviceProvider.GetRequiredService<ISecurityContext>();
            string overrideFilter = criteria.Filter;
            var jArray = new JArray();
            if (IsValidFilter(criteria.Filter))
            {
                var originFilter = JsonConvert.DeserializeObject(criteria.Filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting) as JObject;
                var token = originFilter.Property("and");
                if (token != null)
                {
                    var subArray = (JArray)token.Value;
                    foreach (var item in subArray)
                    {
                        jArray.Add(item);
                    }
                }
                else
                {
                    jArray.Add(originFilter);
                }
            }
            if (!securityContext.FullAccess)
            {
                if (securityContext.RestrictedIds.Any())
                {
                    jArray.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.RestrictedIds), "nin")));
                }
                if (!string.IsNullOrEmpty(securityContext.Upn))
                    jArray.Add(JObject.FromObject(FilterModel.CreateFrom("createdBy", PageSearchType.TEXT, securityContext.Upn, "eq")));

                // origin
                // add the owner into query
                if (securityContext.AllowedIds.Any())
                {
                    var orFilters = new JArray();
                    orFilters.Add(JObject.FromObject(FilterModel.CreateFrom(objectKeyName, objectKeyType, string.Join(",", securityContext.AllowedIds), "in")));

                    if (jArray.HasValues)
                    {
                        // Only add OR condition if the default filter is not null.
                        orFilters.Add(JObject.FromObject(new
                        {
                            and = jArray
                        }));
                    }

                    // IMPORTANCE, change the filter to OR instead of AND:
                    // should be:
                    // OR[{allowIds}, AND[{origin filter},{created_by},{not in restricted}]]
                    var filter = new
                    {
                        or = orFilters
                    };
                    overrideFilter = JsonConvert.SerializeObject(filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
                }
                else if (jArray.Any())
                {
                    // in this case, the filter should be:
                    // AND[{originFilter},{not in restricted},{created_by}]
                    object filter = new
                    {
                        and = jArray
                    };
                    if (jArray.Count == 1)
                    {
                        // no need and, only the object filter
                        filter = jArray.First;
                    }
                    overrideFilter = JsonConvert.SerializeObject(filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
                }
            }
            else if (jArray.Any())
            {
                // in this case, the filter should be:
                // AND[{originFilter},{not in restricted}]
                object filter = new
                {
                    and = jArray
                };
                if (jArray.Count == 1)
                {
                    // no need and, only the object filter
                    filter = jArray.First;
                }
                overrideFilter = JsonConvert.SerializeObject(filter, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            }
            criteria.Filter = overrideFilter;
            return SearchAsync(criteria);
        }

        public virtual async Task<BaseSearchResponse<TResponse>> SearchAsync(TCriteria criteria)
        {
            var start = DateTime.UtcNow;
            IEnumerable<TResponse> data = new List<TResponse>();
            var response = BaseSearchResponse<TResponse>.CreateFrom(criteria, 0, 0, data);
            var tasks = new Task[]{
               RetrieveDataAsync(criteria, response),
               CountAsync(criteria, response)
            };
            await Task.WhenAll(tasks);
            var totalMilisecond = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            response.DurationInMilisecond = (long)totalMilisecond;
            return response;
        }

        protected virtual async Task RetrieveDataAsync(TCriteria criteria, BaseSearchResponse<TResponse> result)
        {
            var requestTenantContext = _serviceProvider.GetRequiredService<ITenantContext>();
            var requestUserContext = _serviceProvider.GetRequiredService<IUserContext>();
            var requestSystemContext = _serviceProvider.GetService<ISystemContext>();
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                //this is important to set the tenant and subtenant new scope.
                var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
                requestTenantContext.CopyTo(tenantContext);

                // need to set user context
                var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
                requestUserContext.CopyTo(userContext);

                var systemContext = scope.ServiceProvider.GetService<ISystemContext>();
                systemContext.SetAppLevel(requestSystemContext.AppLevel);

                var dbType = GetDbType();
                ISearchRepository<TEntity, TKey> repository = scope.ServiceProvider.GetService(dbType) as ISearchRepository<TEntity, TKey>;
                if (repository == null)
                {
                    throw new System.Exception("The repository must be implemented the ISearchRepository");
                }
                IQueryable<TEntity> query = BuildFilter(criteria, repository);

                var idCriteria = JObject.FromObject(criteria).ToObject<TCriteria>();
                idCriteria.Fields = new string[] { "id" };
                IQueryable<TEntity> queryIds = BuildFilter(criteria, repository);
                var ids = await queryIds.AsNoTracking().Select(x => x.Id).Skip(criteria.PageIndex * criteria.PageSize).Take(criteria.PageSize).ToListAsync();
                var listResult = await query.Where(x => ids.Contains(x.Id)).AsNoTracking().ToListAsync();

                if (listResult.Any())
                {
                    result.AddRangeData(listResult.Select(convertToModel));
                }
            }
        }

        protected virtual async Task CountAsync(TCriteria criteria, BaseSearchResponse<TResponse> result)
        {
            var requestTenantContext = _serviceProvider.GetRequiredService<ITenantContext>();
            var requestUserContext = _serviceProvider.GetRequiredService<IUserContext>();
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var requestSystemContext = _serviceProvider.GetService<ISystemContext>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                //this is important to set the tenant and subtenant new scope.
                var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
                requestTenantContext.CopyTo(tenantContext);

                // need to set user context
                var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
                requestUserContext.CopyTo(userContext);

                var systemContext = scope.ServiceProvider.GetService<ISystemContext>();
                systemContext.SetAppLevel(requestSystemContext.AppLevel);

                var dbType = GetDbType();
                ISearchRepository<TEntity, TKey> repository = scope.ServiceProvider.GetService(dbType) as ISearchRepository<TEntity, TKey>;
                if (repository == null)
                {
                    throw new System.Exception("The repository must be implemented the ISearchRepository");
                }
                IQueryable<TEntity> query = BuildFilter(criteria, repository);
                result.TotalCount = await query.CountAsync();
            }
        }

        protected virtual IQueryable<TEntity> BuildFilter(TCriteria criteria, ISearchRepository<TEntity, TKey> repository)
        {
            // var query = IncludeProperties(repository.AsQueryable());
            var query = repository.AsQueryable().AsNoTracking();

            return BuildFilter(criteria, query);
        }

        protected virtual IQueryable<TEntity> BuildFilter(TCriteria criteria, IQueryable<TEntity> query)
        {
            var filterCompiler = _serviceProvider.GetRequiredService<IFilterCompiler>();

            if (IsValidFilter(criteria.Filter))
            {
                var filterObject = JsonConvert.DeserializeObject(criteria.Filter, Constant.JsonSerializerSetting) as JObject;
                if (filterObject == null)
                    throw new ArgumentException($"Filter string is not valid");
                int count = 0;
                var rs = filterCompiler.Compile(filterObject, ref count);
                query = query.Where(rs.Item1, rs.Item2);
            }

            if (!string.IsNullOrEmpty(criteria.Sorts))
            {
                var orders = criteria.Sorts.Split(';').Select(x => x.Replace("=", " "));
                query = query.OrderBy(string.Join(", ", orders));
            }

            if (criteria.Fields != null && criteria.Fields.Any())
            {
                query = query.Select<TEntity>($"new ({string.Join(",", criteria.Fields)})");
            }

            return query;
        }

        protected abstract System.Type GetDbType();

        protected IQueryable<TEntity> IncludeProperties(IQueryable<TEntity> queryable)
        {
            return queryable;
        }

        public virtual async Task<TResponse> FetchAsync(TKey id)
        {
            var dbType = GetDbType();
            var fetchRepository = _serviceProvider.GetService(dbType) as IFetchRepository<TEntity, TKey>;
            if (fetchRepository == null)
            {
                throw new System.Exception("The repository must be implemented the IFetchRepository");
            }
            var entity = await fetchRepository.AsFetchable().Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();
            if (entity != null)
            {
                return convertToModel(entity);
            }
            return null;
        }
        private bool IsValidFilter(string filter)
        {
            return !string.IsNullOrEmpty(filter) && filter != "{}" && filter != "[]";
        }

    }
}
