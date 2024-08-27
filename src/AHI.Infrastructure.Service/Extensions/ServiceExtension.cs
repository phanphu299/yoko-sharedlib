using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Service.Parser;
using AHI.Infrastructure.Service.Builder;
using AHI.Infrastructure.Service.Compiler;
using AHI.Infrastructure.Service.Abstraction;
using AHI.Infrastructure.SharedKernel;
using AHI.Infrastructure.UserContext.Extension;
using AHI.Infrastructure.SystemContext.Extension;

namespace AHI.Infrastructure.Service.Extension
{
    public static class ServiceExtension
    {
        public static void AddFrameworkServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoggingService();
            serviceCollection.AddUserContextService();
            serviceCollection.AddSystemContextService();
            serviceCollection.AddSingleton<IFilterCompiler, SqlFilterCompiler>();
            serviceCollection.AddSingleton<IDictionary<string, IOperationBuilder>>(service =>
            {
                return new Dictionary<string, IOperationBuilder>()
                {
                    { "eq", service.GetRequiredService<EqualsOperationBuilder>() },
                    { "neq", service.GetRequiredService<NotEqualsOperationBuilder>() },
                    { "in", service.GetRequiredService<InOperationBuilder>() },
                    { "nin", service.GetRequiredService<NotInOperationBuilder>() },
                    { "lt", service.GetRequiredService<LessThanOperationBuilder>() },
                    { "lte", service.GetRequiredService<LessThanOrEqualsOperationBuilder>() },
                    { "gt", service.GetRequiredService<GreaterThanOperationBuilder>() },
                    { "gte", service.GetRequiredService<GreaterThanOrEqualsOperationBuilder>() },
                    { "contains", service.GetRequiredService<ContainsOperationBuilder>() },
                    { "ncontains", service.GetRequiredService<NotContainsOperationBuilder>() },
                    { "ago", service.GetRequiredService<AgoOperationBuilder>() },
                    { "between", service.GetRequiredService<BetweenOperationBuilder>() },
                    { "nbetween", service.GetRequiredService<NotBetweenOperationBuilder>() },
                    { "sw", service.GetRequiredService<StartsWithOperationBuilder>() },
                    { "nsw", service.GetRequiredService<NotStartsWithOperationBuilder>() },
                    { "ew", service.GetRequiredService<EndsWithOperationBuilder>() },
                    { "new", service.GetRequiredService<NotEndsWithOperationBuilder>() },
                };
            });
            serviceCollection.AddSingleton<EqualsOperationBuilder>();
            serviceCollection.AddSingleton<NotEqualsOperationBuilder>();
            serviceCollection.AddSingleton<InOperationBuilder>();
            serviceCollection.AddSingleton<NotInOperationBuilder>();
            serviceCollection.AddSingleton<LessThanOperationBuilder>();
            serviceCollection.AddSingleton<LessThanOrEqualsOperationBuilder>();
            serviceCollection.AddSingleton<GreaterThanOperationBuilder>();
            serviceCollection.AddSingleton<GreaterThanOrEqualsOperationBuilder>();
            serviceCollection.AddSingleton<ContainsOperationBuilder>();
            serviceCollection.AddSingleton<NotContainsOperationBuilder>();
            serviceCollection.AddSingleton<AgoOperationBuilder>();
            serviceCollection.AddSingleton<BetweenOperationBuilder>();
            serviceCollection.AddSingleton<NotBetweenOperationBuilder>();
            serviceCollection.AddSingleton<StartsWithOperationBuilder>();
            serviceCollection.AddSingleton<NotStartsWithOperationBuilder>();
            serviceCollection.AddSingleton<EndsWithOperationBuilder>();
            serviceCollection.AddSingleton<NotEndsWithOperationBuilder>();
            serviceCollection.AddSingleton<IValueParser<string>, StringParser>();
            serviceCollection.AddSingleton<IValueParser<double>, NumbericParser>();
            serviceCollection.AddSingleton<IValueParser<bool>, BoolParser>();
            serviceCollection.AddSingleton<IValueParser<Guid>, GuidParser>();
            serviceCollection.AddSingleton<IValueArrayParser<Guid>, GuidArrayParser>();
            serviceCollection.AddSingleton<IValueParser<DateTime>, DateTimeParser>();
            serviceCollection.AddSingleton<IValueArrayParser<string>, StringArrayParser>();
            serviceCollection.AddSingleton<IValueArrayParser<double>, NumbericArrayParser>();
            serviceCollection.AddSingleton<IValueArrayParser<DateTime>, DateTimeArrayParser>();
        }
    }
}