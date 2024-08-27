using System;
using AHI.Infrastructure.Service.Dapper.Model;

namespace AHI.Infrastructure.Service.Dapper.Abstraction
{
    public delegate (string Query, object[] Values, string[] Tokens) OperationBuilder(QueryFilter filter, Action<string[]> callback = null);
    public interface IOperationBuilder
    {
        (string Query, object[] Values, string[] Tokens) Build(QueryFilter filter, Action<string[]> callback = null);
    }
}