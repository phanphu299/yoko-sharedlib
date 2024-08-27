using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Service.Dapper.Parser;
using AHI.Infrastructure.Service.Dapper.Builder;
using AHI.Infrastructure.Service.Dapper.Compiler;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.SharedKernel;
using AHI.Infrastructure.UserContext.Extension;
using AHI.Infrastructure.SystemContext.Extension;
using AHI.Infrastructure.Service.Dapper.Constant;

namespace AHI.Infrastructure.Service.Dapper.Extension
{
    public static class ServiceExtension
    {
        public static void AddDapperFrameworkServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoggingService();
            serviceCollection.AddUserContextService();
            serviceCollection.AddSystemContextService();
            serviceCollection.AddSingleton<IFilterCompiler, SqlFilterCompiler>();
            serviceCollection.AddSingleton<IDictionary<string, IOperationBuilder>>(service =>
            {
                return new Dictionary<string, IOperationBuilder>()
                {
                    { Operation.EQUALS, service.GetRequiredService<EqualsOperationBuilder>() },
                    { Operation.NOT_EQUALS, service.GetRequiredService<NotEqualsOperationBuilder>() },
                    { Operation.IN, service.GetRequiredService<InOperationBuilder>() },
                    { Operation.NOT_IN, service.GetRequiredService<NotInOperationBuilder>() },
                    { Operation.LESS_THAN, service.GetRequiredService<LessThanOperationBuilder>() },
                    { Operation.LESS_THAN_OR_EQUALS, service.GetRequiredService<LessThanOrEqualsOperationBuilder>() },
                    { Operation.GREATER_THAN, service.GetRequiredService<GreaterThanOperationBuilder>() },
                    { Operation.GREATER_THAN_OR_EQUALS, service.GetRequiredService<GreaterThanOrEqualsOperationBuilder>() },
                    { Operation.CONTAINS, service.GetRequiredService<ContainsOperationBuilder>() },
                    { Operation.NOT_CONTAINS, service.GetRequiredService<NotContainsOperationBuilder>() },
                    { Operation.BETWEEN, service.GetRequiredService<BetweenOperationBuilder>() },
                    { Operation.NOT_BETWEEN, service.GetRequiredService<NotBetweenOperationBuilder>() },
                    { Operation.STARTS_WITH, service.GetRequiredService<StartsWithOperationBuilder>() },
                    { Operation.NOT_STARTS_WITH, service.GetRequiredService<NotStartsWithOperationBuilder>() },
                    { Operation.ENDS_WITH, service.GetRequiredService<EndsWithOperationBuilder>() },
                    { Operation.NOT_ENDS_WITH, service.GetRequiredService<NotEndsWithOperationBuilder>() },
                    { Operation.NULL, service.GetRequiredService<NullOperationBuilder>() },
                    { Operation.LTREE_QUERRY, service.GetRequiredService<LQueryOperationBuilder>() },
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
            serviceCollection.AddSingleton<BetweenOperationBuilder>();
            serviceCollection.AddSingleton<NotBetweenOperationBuilder>();
            serviceCollection.AddSingleton<StartsWithOperationBuilder>();
            serviceCollection.AddSingleton<NotStartsWithOperationBuilder>();
            serviceCollection.AddSingleton<EndsWithOperationBuilder>();
            serviceCollection.AddSingleton<NotEndsWithOperationBuilder>();
            serviceCollection.AddSingleton<NullOperationBuilder>();
            serviceCollection.AddSingleton<LQueryOperationBuilder>();
            serviceCollection.AddSingleton<IValueParser<string>, StringParser>();
            serviceCollection.AddSingleton<IValueParser<double>, NumbericParser>();
            serviceCollection.AddSingleton<IValueParser<bool>, BoolParser>();
            serviceCollection.AddSingleton<IValueParser<Guid>, GuidParser>();
            serviceCollection.AddSingleton<IValueParser<DateTime>, DateTimeParser>();
            serviceCollection.AddSingleton<IValueArrayParser<Guid>, GuidArrayParser>();
            serviceCollection.AddSingleton<IValueArrayParser<string>, StringArrayParser>();
            serviceCollection.AddSingleton<IValueArrayParser<double>, NumbericArrayParser>();
            serviceCollection.AddSingleton<IValueArrayParser<DateTime>, DateTimeArrayParser>();
            serviceCollection.AddScoped<IQueryService, DapperQueryService>();
        }
    }
}