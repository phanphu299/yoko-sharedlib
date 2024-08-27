using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.Service.Dapper.Abstraction
{
    public interface IFilterCompiler
    {
        (string Query, ExpandoObject Value) Compile(JObject filter, ref int count);
    }
}