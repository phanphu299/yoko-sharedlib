using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.Service.Model;
using AHI.Infrastructure.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
namespace AHI.Infrastructure.Service.Compiler
{
    public class SqlFilterCompiler : IFilterCompiler
    {
        protected IDictionary<string, IOperationBuilder> supportOperations;

        public SqlFilterCompiler(IDictionary<string, IOperationBuilder> supportOperations)
        {
            this.supportOperations = supportOperations;
        }

        public Tuple<string, object[], string[]> Compile(JObject filter, ref int count, Action<string[]> callback = null)
        {
            if (filter.Count != 1)
            {
                var filterModel = filter.ToObject<FilterModel>(Constant.JsonSerializer);
                if (!supportOperations.ContainsKey(filterModel.Operation)) throw new NotSupportedException($"{filterModel.Operation} is not supported");
                var builder = supportOperations[filterModel.Operation];
                var item = builder.Build(filterModel);
                var q = item.Item1;
                var listMap = new List<string>();
                for (int i = 0; i < item.Item2.Length; i++)
                {
                    var index = $"@{count}";
                    q = q.Replace(item.Item3[i], index);
                    listMap.Add(index);
                    count++;
                }
                return new Tuple<string, object[], string[]>(q, item.Item2, listMap.ToArray());
            }
            else
            {
                var dictionary = filter.ToObject<IDictionary<string, JArray>>(Constant.JsonSerializer);
                var listQuery = new List<string>();
                var listObject = new List<object>();
                var listMap = new List<string>();
                string queryResult = string.Empty;
                foreach (var item in dictionary)
                {
                    foreach (var value in item.Value)
                    {
                        var rs = Compile(value as JObject, ref count, callback);
                        if (rs.Item2.Length != rs.Item3.Length) throw new System.Exception("Wrong index");
                        listObject.AddRange(rs.Item2);
                        listMap.AddRange(rs.Item3);
                        listQuery.Add(rs.Item1);
                    }
                    queryResult = $"( {string.Join($" {item.Key.TrimStart('$')} ", listQuery)} )";
                }
                return new Tuple<string, object[], string[]>(queryResult, listObject.ToArray(), listMap.ToArray());
            }
        }
    }
}