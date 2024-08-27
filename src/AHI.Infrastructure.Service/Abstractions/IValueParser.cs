namespace AHI.Infrastructure.Service.Abstraction
{
    public interface IValueParser<T>
    {
        T Parse(string value);
    }
}