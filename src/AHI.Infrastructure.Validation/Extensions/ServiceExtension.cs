using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Validation.Abstraction;
using AHI.Infrastructure.Validation.Repository.Abstraction;
using AHI.Infrastructure.Validation.Repository;
using AHI.Infrastructure.Validation.Services;
using AHI.Infrastructure.Validation.Services.TypeValidators;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel;
namespace AHI.Infrastructure.Validation.Extension
{
    public static class ServiceExtension
    {
        #region Methods

        public static void AddDynamicValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoggingService();
            serviceCollection.AddScoped<IDynamicValidationRepository, SystemConfigRepository>();
            serviceCollection.AddScoped<IDynamicValidator, DynamicValidator>();
            serviceCollection.AddScoped<ITypeValidator>(service =>
            {
                var fieldRepository = service.GetRequiredService<IDynamicValidationRepository>();
                var stringValidatorLogger = service.GetRequiredService<ILoggerAdapter<BaseTypeValidator>>();
                var stringValidator = new StringValidator(fieldRepository, null, stringValidatorLogger);
                return stringValidator;
            });
        }

        #endregion
    }
}