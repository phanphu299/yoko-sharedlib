using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AHI.Infrastructure.Interceptor.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;

namespace AHI.Infrastructure.Interceptor
{
    public class DynamicResolver : IDynamicResolver
    {
        private readonly IDictionary<string, Assembly> _interceptors;
        private readonly ICompilerService _compilerService;

        public DynamicResolver(ICompilerService compilerService)
        {
            _interceptors = new ConcurrentDictionary<string, Assembly>();
            _compilerService = compilerService;
        }

        public BaseInterceptor ResolveInstance(string condition, string action = "return true", string usingNamespace = "")
        {
            var contentMd5 = action.CalculateMd5Hash();
            if (!_interceptors.ContainsKey(contentMd5))
            {
                // build the engine code here
                var template = @"using System;
                            using System.Linq;
                            using System.Collections.Generic;
                            {{import}}
                            using AHI.Infrastructure.Interceptor.Abstraction;
                            namespace AHI.Infrastructure.Interceptor
                            {
                                public class ConcreteInterceptor : BaseInterceptor
                                {
                                   // public ConcreteInterceptor(IServiceProvider serviceProvider) : base(serviceProvider) { }
                                    public override object OnApply(IDictionary<string, object> request)
                                    {
                                        if (CanApply(request))
                                        {
                                            return Apply(request);
                                        }
                                        return null;
                                    }
                                    private bool CanApply(IDictionary<string, object> request)
                                    {
                                        // this condition will take place
                                        {{condition}};
                                    }
                                    private object Apply(IDictionary<string, object> request)
                                    {
                                         // action for this interceptor
                                        {{action}};
                                    }
                                }
                            }".Replace("{{condition}}", condition).Replace("{{action}}", action).Replace("{{import}}", usingNamespace);
                _interceptors[contentMd5] = _compilerService.CompileToAssembly(contentMd5, template); ;
            }
            var assembly = _interceptors[contentMd5];
            return CreateIntance(assembly);
        }
        private BaseInterceptor CreateIntance(Assembly assembly)
        {
            var type = assembly.GetTypes().FirstOrDefault(t => typeof(BaseInterceptor).IsAssignableFrom(t));
            return Activator.CreateInstance(type) as BaseInterceptor;
        }
    }
}