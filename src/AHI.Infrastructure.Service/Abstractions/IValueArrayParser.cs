namespace AHI.Infrastructure.Service.Abstraction
{
    public interface IValueArrayParser<T>
    {
        T[] Parse(string value);
    }
}