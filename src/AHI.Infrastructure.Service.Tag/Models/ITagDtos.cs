using System.Collections.Generic;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public interface ITagDtos
    {
        List<TagDto> Tags { get; set; }
    }
}