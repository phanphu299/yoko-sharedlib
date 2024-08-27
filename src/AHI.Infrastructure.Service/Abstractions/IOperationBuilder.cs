using System;
using AHI.Infrastructure.Service.Model;

namespace AHI.Infrastructure.Service.Abstraction
{
    public delegate Tuple<string, object[], string[]> OperationBuilder(FilterModel filter, Action<string[]> callback = null);
    public interface IOperationBuilder
    {
        Tuple<string, object[], string[]> Build(FilterModel filter, Action<string[]> callback = null);
    }
}