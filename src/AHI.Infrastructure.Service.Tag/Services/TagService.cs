using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Service.Tag.Configuration;
using AHI.Infrastructure.Service.Tag.Constant;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Extension;
using System.Text;
using Newtonsoft.Json;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.Exception;
using System.Linq;
using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.UserContext.Abstraction;

namespace AHI.Infrastructure.Service.Tag.Service
{
    public class TagService : ITagService
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerAdapter<TagService> _logger;

        public TagService(ITenantContext tenantContext, IUserContext userContext, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILoggerAdapter<TagService> logger)
        {
            _tenantContext = tenantContext;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<long[]> UpsertTagsAsync(IUpsertTagCommand upsertTagCommand)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceTagConstants.HTTP_CLIENT_NAME, _tenantContext);
            upsertTagCommand.Upn = _userContext.Upn;
            if (upsertTagCommand.ApplicationId == Guid.Empty && !string.IsNullOrEmpty(_userContext.ApplicationId))
            {
                upsertTagCommand.ApplicationId = Guid.Parse(_userContext.ApplicationId);
            }
            var payload = upsertTagCommand;
            _logger.LogDebug($"UpsertTagsAsync payload: {payload.ToJson()}");
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PatchAsync($"tag/tags", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"UpsertTagsAsync - Request body: {payload.ToJson()}");
                await HandleUnsuccessResponseMessageAsync(response, ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            var contentStream = await response.Content.ReadAsByteArrayAsync();
            var data = contentStream.Deserialize<long[]>();
            return data;
        }

        public async Task<TagDto[]> UpsertTagsV2Async(IUpsertTagCommand upsertTagCommand)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceTagConstants.HTTP_CLIENT_NAME, _tenantContext);
            upsertTagCommand.Upn = _userContext.Upn;
            if (upsertTagCommand.ApplicationId == Guid.Empty && !string.IsNullOrEmpty(_userContext.ApplicationId))
            {
                upsertTagCommand.ApplicationId = Guid.Parse(_userContext.ApplicationId);
            }

            var payload = upsertTagCommand;
            _logger.LogDebug($"UpsertTagsV2Async payload: {payload.ToJson()}");
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PatchAsync($"tag/tags/v2", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"UpsertTagsV2Async - Request body: {payload.ToJson()}");
                await HandleUnsuccessResponseMessageAsync(response, ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }

            var contentStream = await response.Content.ReadAsByteArrayAsync();
            var data = contentStream.Deserialize<TagDto[]>();

            return data;
        }

        public async Task DeleteTagsAsync(long[] tagIds)
        {
            var query = string.Empty;

            switch (DbConfig.DatabaseType)
            {
                case Enum.DatabaseType.Postgresql:
                    query = "DELETE FROM entity_tags WHERE tag_id = ANY(@TagIds);";
                    break;
                case Enum.DatabaseType.SqlServer:
                    query = "DELETE FROM entity_tags WHERE tag_id IN @TagIds;";
                    break;
                default:
                    throw new NotImplementedException();
            }

            _logger.LogDebug($"DeleteTagsAsync query: {query}, tagids: {tagIds.ToJson()}");

            using (var sqlConnection = GetDbConnection(_configuration, _tenantContext))
            {
                try
                {
                    await sqlConnection.ExecuteAsync(query, new { TagIds = tagIds });
                }
                catch (System.Exception ex)
                {
                    if (!(ex.Message.Contains("database") && ex.Message.Contains("does not exist")))
                        throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public Task DeleteTagsAsync(IDeleteTagMessage deleteTagMessage)
        {
            return DeleteTagsAsync(deleteTagMessage.TagIds);
        }

        private IDbConnection GetDbConnection(IConfiguration configuration, ITenantContext tenantContext)
        {
            var connectionString = configuration["ConnectionStrings:Default"].BuildConnectionString(configuration, tenantContext.ProjectId);
            IDbConnection connection = null;
            switch (DbConfig.DatabaseType)
            {
                case Enum.DatabaseType.Postgresql:
                    connection = new NpgsqlConnection(connectionString);
                    break;
                case Enum.DatabaseType.SqlServer:
                    connection = new SqlConnection(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return connection;
        }

        private async Task HandleUnsuccessResponseMessageAsync(HttpResponseMessage response, string defaultMessage = null)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            _logger.LogError($"HandleUnsuccessResponseMessageAsync - Received response: StatusCode: {response.StatusCode} - ReasonPhrase: {response.ReasonPhrase} - Details: {jsonString}");
            var httpResponse = jsonString.FromJson<HttpResponseError>();
            if (httpResponse == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }

            throw httpResponse.GenerateException(defaultMessage);
        }

        public async Task<BaseSearchResponse<T>> FetchTagsAsync<T>(BaseSearchResponse<T> searchResponse) where T : ITagDtos
        {
            if (searchResponse.Data == null)
                return searchResponse;

            var tagIds = searchResponse.Data.Where(x => x.Tags != null).SelectMany(x => x.Tags).Select(x => x.Id).Distinct();

            if (!tagIds.Any())
                return searchResponse;

            var responseData = await FetchTagsAsync(tagIds);

            if (responseData.Any())
            {
                foreach (var data in searchResponse.Data)
                {
                    data.Tags = (from t in data.Tags
                                 join r in responseData on t.Id equals r.Id
                                 select r).ToList();
                }
            }
            return searchResponse;
        }

        public async Task<IEnumerable<T>> FetchTagsAsync<T>(IEnumerable<T> resultTag) where T : ITagDtos
        {
            if (resultTag == null)
            {
                resultTag = Array.Empty<T>();
            }
            var tagIds = resultTag.Where(x => x.Tags != null).SelectMany(x => x.Tags).Select(x => x.Id).Distinct();

            if (!tagIds.Any())
                return resultTag;

            var responseData = await FetchTagsAsync(tagIds);
            if (responseData.Any())
            {
                foreach (var data in resultTag)
                {
                    data.Tags = (from t in data.Tags
                                 join r in responseData on t.Id equals r.Id
                                 select r).ToList();
                }
            }
            return resultTag;
        }

        public async Task<T> FetchTagsAsync<T>(T resultTag) where T : ITagDtos
        {
            if (resultTag.Tags == null)
                return resultTag;

            var tagIds = resultTag.Tags.Select(x => x.Id).Distinct();

            if (!tagIds.Any())
                return resultTag;

            var responseData = await FetchTagsAsync(tagIds);
            if (responseData.Any())
            {
                resultTag.Tags = responseData.ToList();
            }
            return resultTag;
        }

        public async Task<IEnumerable<TagDto>> FetchTagsAsync(IEnumerable<long> tagIds)
        {
            if (!tagIds.Any())
                return Array.Empty<TagDto>();

            var httpClient = _httpClientFactory.CreateClient(ServiceTagConstants.HTTP_CLIENT_NAME, _tenantContext);
            var content = new StringContent(tagIds.ToJson(), Encoding.UTF8, mediaType: "application/json");
            var responseMessage = await httpClient.PostAsync($"tag/tags/fetch", content);
            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"FetchTagsAsync - Request body: {tagIds.ToJson()}");
                await HandleUnsuccessResponseMessageAsync(responseMessage, ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            var streamData = await responseMessage.Content.ReadAsByteArrayAsync();
            var response = streamData.Deserialize<IEnumerable<TagDto>>();
            return response;
        }
    }
}