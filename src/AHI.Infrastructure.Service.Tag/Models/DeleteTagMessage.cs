namespace AHI.Infrastructure.Service.Tag.Model
{
    public class DeleteTagMessage : IDeleteTagMessage
    {
        public long[] TagIds { get; set; }
    }
}