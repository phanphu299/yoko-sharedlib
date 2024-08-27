using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Interceptor.Abstraction;

namespace AHI.Infrastructure.Interceptor.Extension
{
    public static class ServiceExtension
    {
        public static void AddInterceptor(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDynamicResolver, DynamicResolver>();
            serviceCollection.AddSingleton<ICompilerService, CompilerService>();
            serviceCollection.AddSingleton<ILanguageService, CSharpLanguage>();
        }
    }
}