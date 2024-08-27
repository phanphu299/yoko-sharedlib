namespace AHI.Infrastructure.Service.Tag.Model
{
    public class TagDto : ITagDto
    {
        public TagDto(long id, string key, string value)
        {
            Id = id;
            Key = key;
            Value = value;
        }

        public long Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}