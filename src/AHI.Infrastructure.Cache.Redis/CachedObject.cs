namespace AHI.Infrastructure.Cache.Redis
{
    public class CachedObject<T>
    {
        public T Value { get; set; }
        public string Key { get; set; }

        public CachedObject(T value)
        {
            Value = value;
        }
    }
}
