namespace AHI.Infrastructure.Service.Dapper.Constant
{
    public static class Operation
    {
        public static string EQUALS = "eq";
        public static string NOT_EQUALS = "neq";
        public static string IN = "in";
        public static string NOT_IN = "nin";
        public static string LESS_THAN = "lt";
        public static string LESS_THAN_OR_EQUALS = "lte";
        public static string GREATER_THAN = "gt";
        public static string GREATER_THAN_OR_EQUALS = "gte";
        public static string CONTAINS = "contains";
        public static string NOT_CONTAINS = "ncontains";
        public static string BETWEEN = "between";
        public static string NOT_BETWEEN = "nbetween";
        public static string STARTS_WITH = "sw";
        public static string NOT_STARTS_WITH = "nsw";
        public static string ENDS_WITH = "ew";
        public static string NOT_ENDS_WITH = "new";
        public static string NULL = "null";
        public static string LTREE_QUERRY = "lqr";
    }

    public static class LogicalOperator
    {
        public static string AND = "and";
        public static string OR = "or";
    }
}