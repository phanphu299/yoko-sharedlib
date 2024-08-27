using System.Collections.Generic;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public class TagDtos : ITagDtos
    {
        public List<TagDto> Tags { get; set; }
    }
}