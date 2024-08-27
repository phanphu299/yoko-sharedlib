using AHI.Infrastructure.Service.Tag.Model;
using System.Collections.Generic;
using System.Linq;

namespace AHI.Infrastructure.Service.Tag.Extension
{
    public static class TagDtoExtension
    {
        public static List<TagDto> MappingTagDto<T>(this ICollection<T> entityTagDbs) where T : IEntityTag
        {
            return entityTagDbs != null ? entityTagDbs.OrderBy(x => x.Id).Select(x => new TagDto(x.TagId, null, null)).ToList() : new List<TagDto>();
        }
    }
}