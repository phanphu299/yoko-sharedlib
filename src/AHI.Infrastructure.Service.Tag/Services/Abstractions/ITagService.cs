using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.SharedKernel.Model;

namespace AHI.Infrastructure.Service.Tag.Service.Abstraction
{
    public interface ITagService
    {
        Task<long[]> UpsertTagsAsync(IUpsertTagCommand upsertTagCommand);
        Task<TagDto[]> UpsertTagsV2Async(IUpsertTagCommand upsertTagCommand);
        Task DeleteTagsAsync(long[] tagIds);
        Task DeleteTagsAsync(IDeleteTagMessage deleteTagMessage);
        Task<BaseSearchResponse<T>> FetchTagsAsync<T>(BaseSearchResponse<T> searchResponse) where T : ITagDtos;
        Task<IEnumerable<T>> FetchTagsAsync<T>(IEnumerable<T> resultTag) where T : ITagDtos;
        Task<T> FetchTagsAsync<T>(T resultTag) where T : ITagDtos;
        Task<IEnumerable<TagDto>> FetchTagsAsync(IEnumerable<long> tagIds);
    }
}