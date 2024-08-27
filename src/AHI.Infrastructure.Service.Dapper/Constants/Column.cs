using System.Linq;
using System.Text.RegularExpressions;

namespace AHI.Infrastructure.Service.Dapper.Constant
{
    public static class Column
    {
        public const string ORDER_BY_ASC = "asc";
        public const string ORDER_BY_DESC = "desc";
        public const string ValidColumnNameRegex = "^[a-zA-Z0-9_]+$";
        public static readonly string[] ORDER_BY_TYPES = { ORDER_BY_ASC, ORDER_BY_DESC };
        public static bool HasValidOrderByType(string orderByType) => ORDER_BY_TYPES.Contains(orderByType);
        public static bool HasValidName(string name) => new Regex(ValidColumnNameRegex).IsMatch(name);
    }
}