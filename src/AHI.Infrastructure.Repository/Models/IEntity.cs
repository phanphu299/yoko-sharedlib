namespace AHI.Infrastructure.Repository.Model.Generic
{
    public interface IEntity<T>
    {
        T Id { get; }
    }
}
