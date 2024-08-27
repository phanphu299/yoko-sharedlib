using System;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.Service.Dapper.Model;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using System.Linq;
using AHI.Infrastructure.Service.Dapper.Helpers;
using AHI.Infrastructure.Service.Dapper.Constant;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Compiler
{
    public class SqlFilterCompiler : IFilterCompiler
    {
        protected IDictionary<string, IOperationBuilder> supportOperations;

        public SqlFilterCompiler(IDictionary<string, IOperationBuilder> supportOperations)
        {
            this.supportOperations = supportOperations;
        }

        public (string Query, ExpandoObject Value) Compile(JObject filter, ref int count)
        {
            //Case dont have logical operator
            if (!filter.HasLogicalOperator())
            {
                var queryFilter = filter.ToObject<QueryFilter>(SharedKernel.Extension.Constant.JsonSerializer);

                if (string.IsNullOrEmpty(queryFilter.QueryKey))
                    throw new ArgumentException($"QueryKey value is required");

                // can be the method in property like name.ToLower() or something.
                // if (!Column.HasValidName(queryFilter.QueryKey))
                //     throw new NotSupportedException($"{queryFilter.QueryKey} is invalid");

                if (!supportOperations.ContainsKey(queryFilter.Operation))
                    throw new NotSupportedException($"{queryFilter.Operation} is not supported");

                queryFilter.QueryKey = queryFilter.QueryKey.ToColumnStringName();

                var builder = supportOperations[queryFilter.Operation];
                var result = builder.Build(queryFilter);
                var query = result.Query;
                var objValue = new ExpandoObject();
                for (int i = 0; i < result.Values.Length; i++)
                {
                    var valueIndex = $"@{count}";
                    query = query.Replace(result.Tokens[i], valueIndex);
                    objValue.TryAdd(count.ToString(), result.Values[i]);
                    count++;
                }
                return (query, objValue);
            }
            else
            {
                var dictionary = filter.ToObject<IDictionary<string, JArray>>(SharedKernel.Extension.Constant.JsonSerializer);
                var listQuery = new List<string>();
                var queryResult = string.Empty;
                var objValue = new ExpandoObject();
                foreach (var item in dictionary.Where(x => x.Value != null))
                {
                    foreach (var value in item.Value)
                    {
                        var result = Compile(value as JObject, ref count);
                        listQuery.Add(result.Query);
                        foreach (var r in result.Value)
                        {
                            objValue.TryAdd(r.Key, r.Value);
                        }
                    }
                    queryResult = $"( {string.Join($" {item.Key.TrimStart('$')} ", listQuery)} )";
                }
                return (queryResult, objValue);
            }
        }
    }
}