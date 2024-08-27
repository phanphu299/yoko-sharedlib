using System;
using System.Collections.Generic;

namespace AHI.Infrastructure.Interceptor.Abstraction
{
    public abstract class BaseInterceptor
    {

        // protected IServiceProvider _serviceProvider;

        // public BaseInterceptor(IServiceProvider serviceProvider)
        // {
        //     _serviceProvider = serviceProvider;
        // }

        public abstract object OnApply(IDictionary<string, object> request);
    }
}
