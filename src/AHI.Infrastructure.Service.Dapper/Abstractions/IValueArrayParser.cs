namespace AHI.Infrastructure.Service.Dapper.Abstraction
{
    public interface IValueArrayParser<T>
    {
        T[] Parse(string value);
    }
}