using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.Validation.Constants;
using AHI.Infrastructure.Validation.Model;
using AHI.Infrastructure.Validation.Repository.Abstraction;

namespace AHI.Infrastructure.Validation.Services.TypeValidators
{
    public class StringValidator : BaseTypeValidator
    {
        #region Properties

        private readonly BaseTypeValidator _next;

        #endregion

        #region Constructor

        public StringValidator(IDynamicValidationRepository dynamicValidationRepository, BaseTypeValidator next,
            ILoggerAdapter<BaseTypeValidator> logger) : base(dynamicValidationRepository, logger)
        {
            _next = next;
        }

        #endregion

        #region Methods

        protected override async Task<ValidationFailure[]> ValidatePropertyAsync(PropertyValidationContext context, PropertyValidationRule validationRule, CancellationToken cancellationToken = default)
        {
            var text = context.Value as string;
            if (context.ValueType != typeof(string))
            {
                if (_next == null)
                    return Array.Empty<ValidationFailure>();

                await _next?.ValidateAsync(context, cancellationToken);
            }

            // Field must be required.
            if (string.IsNullOrEmpty(text))
            {
                if (validationRule.IsRequired)
                {
                    var validationFailure = new ValidationFailure(context.PropertyName,
                        $"{context.PropertyName} is required", text);
                    validationFailure.ErrorCode = ValidationErrorCodes.PropertyIsRequired;
                    return new[] { validationFailure };
                }

                return Array.Empty<ValidationFailure>();
            }

            // Regex does not match.
            if (!string.IsNullOrWhiteSpace(validationRule.Regex) && !Regex.IsMatch(text, validationRule.Regex))
            {
                var validationFailure = new ValidationFailure(context.PropertyName, validationRule.Description, text);
                validationFailure.ErrorCode = ValidationErrorCodes.RegexNotMatch;
                return new[] { validationFailure };
            }

            // Maximum length exceeded.
            if (validationRule.MaxLength != null && text.Length > validationRule.MaxLength)
            {
                var validationFailure = new ValidationFailure(context.PropertyName, validationRule.Description, text);
                validationFailure.ErrorCode = ValidationErrorCodes.MaxLengthExceeded;

                return new[] { validationFailure };
            }

            return Array.Empty<ValidationFailure>();
        }

        #endregion
    }
}