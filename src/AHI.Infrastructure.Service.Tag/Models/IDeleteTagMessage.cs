namespace AHI.Infrastructure.Service.Tag.Model
{
    public interface IDeleteTagMessage
    {
        long[] TagIds { get; set; }
    }
}