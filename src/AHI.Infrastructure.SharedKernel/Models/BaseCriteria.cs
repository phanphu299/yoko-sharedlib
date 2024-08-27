namespace AHI.Infrastructure.SharedKernel.Model
{
    public class BaseCriteria
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string Filter { get; set; }
        public string Sorts { get; set; } = "id=desc";
        public string[] Fields { get; set; }
    }
}