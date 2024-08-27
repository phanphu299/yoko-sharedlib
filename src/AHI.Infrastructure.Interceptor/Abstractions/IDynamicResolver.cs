namespace AHI.Infrastructure.Interceptor.Abstraction
{
    public interface IDynamicResolver
    {
        BaseInterceptor ResolveInstance(string condition, string action = "return true", string usingNamespace = "");
    }
}
