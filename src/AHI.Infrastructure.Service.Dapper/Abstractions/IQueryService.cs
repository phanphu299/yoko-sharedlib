using System.Dynamic;
using AHI.Infrastructure.Service.Dapper.Model;
using Dapper;

namespace AHI.Infrastructure.Service.Dapper.Abstraction
{
    public interface IQueryService
    {
        (string Query, ExpandoObject Value) CompileQuery(string query, QueryCriteria queryCriteria, bool paging = true);
        (string Paging, ExpandoObject Value) CompileQuery(SqlBuilder sqlBuilder, QueryCriteria queryCriteria, bool paging=true);
    }
}