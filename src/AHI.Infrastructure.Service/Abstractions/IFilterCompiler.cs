using System;
using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.Service.Abstraction
{
    public interface IFilterCompiler
    {
        Tuple<string, object[], string[]> Compile(JObject filter, ref int count, Action<string[]> callback = null);
    }
}