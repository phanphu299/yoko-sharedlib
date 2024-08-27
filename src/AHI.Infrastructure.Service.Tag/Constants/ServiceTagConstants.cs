namespace AHI.Infrastructure.Service.Tag.Constant
{
    public static class ServiceTagConstants
    {
        public const string SEARCH_QUERY_KEY = "EntityTags.TagId";
        public const string QUERY_TYPE = "text";
        public const string TAG_SELECT_FIELD = "EntityTags";
        public const string HTTP_CLIENT_NAME = "EntityTag";
        public const string TAG_ENDPOINT_CONFIGURATION = "Api:Tag";
        public static string[] ShowRecordDontHaveTagIfOperationNotMatch = new string[] { "neq", "nin", "ncontains", "nbetween", "nsw", "new" };
    }
}