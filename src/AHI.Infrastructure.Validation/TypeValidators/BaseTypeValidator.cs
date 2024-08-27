using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.Validation.Abstraction;
using AHI.Infrastructure.Validation.Model;
using AHI.Infrastructure.Validation.Repository.Abstraction;

namespace AHI.Infrastructure.Validation
{
    public abstract class BaseTypeValidator : ITypeValidator
    {
        #region Properties

        private readonly IDynamicValidationRepository _dynamicValidationRepository;

        private readonly ILoggerAdapter<BaseTypeValidator> _logger;

        #endregion

        #region Constructor

        protected BaseTypeValidator()
        {

        }

        protected BaseTypeValidator(IDynamicValidationRepository dynamicValidationRepository, ILoggerAdapter<BaseTypeValidator> logger) : this()
        {
            _dynamicValidationRepository = dynamicValidationRepository;
            _logger = logger;
        }

        #endregion

        #region Methods

        public virtual async Task<ValidationFailure[]> ValidateAsync(PropertyValidationContext context, CancellationToken cancellationToken = default)
        {
            if (context == null)
                return Array.Empty<ValidationFailure>();

            var attributes = context.GetAttributes();
            if (attributes == null || !attributes.Any())
                return Array.Empty<ValidationFailure>();

            // Get the validation keys.
            var keys = attributes.Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .Select(x => x.Key)
                .Distinct();

            // No key is defined.
            if (!keys.Any())
                return Array.Empty<ValidationFailure>();

            // Get the validation rules.
            var validationRules =
                await _dynamicValidationRepository.GetValidationRulesAsync(keys);

            var validationFailures = new LinkedList<ValidationFailure>();

            foreach (var attribute in attributes)
            {
                var validationRule = validationRules.FirstOrDefault(x => x.Key.Equals(attribute.Key, System.StringComparison.InvariantCultureIgnoreCase));
                if (validationRule == null)
                    continue;

                var localValidationFailures = await ValidatePropertyAsync(context, validationRule, cancellationToken);
                if (localValidationFailures.Any())
                {
                    foreach (var validationFailure in localValidationFailures)
                        validationFailures.AddLast(validationFailure);
                }
            }

            return validationFailures.ToArray();
        }

        protected abstract Task<ValidationFailure[]> ValidatePropertyAsync(PropertyValidationContext context, PropertyValidationRule validationRule, CancellationToken cancellationToken = default);

        #endregion
    }
}