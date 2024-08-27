namespace AHI.Infrastructure.MultiTenancy.Option
{
    public class MultiTenancyOption
    {
        public string[] SkipPaths { get; set; }
        public string[] ExcludeWhenQueryPresentValues { get; set; }
    }
}