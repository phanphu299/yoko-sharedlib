namespace AHI.Infrastructure.Service.Dapper.Abstraction
{
    public interface IValueParser<T>
    {
        T Parse(string value);
    }
}