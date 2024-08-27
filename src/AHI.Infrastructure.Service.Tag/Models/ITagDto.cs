namespace AHI.Infrastructure.Service.Tag.Model
{
    public interface ITagDto
    {
        long Id { get; set; }
        string Key { get; set; }
        string Value { get; set; }
    }
}