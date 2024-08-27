using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace AHI.Infrastructure.Exception
{
    public class EntityValidationException : BaseException
    {
        public IDictionary<string, string[]> Failures { get; }
        public IDictionary<string, object> ValidationInfo { get; }

        public EntityValidationException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_VALIDATION, message, detailCode, payload, innerException)
        {
            Failures = new Dictionary<string, string[]>();
            ValidationInfo = new Dictionary<string, object>();
        }

        public EntityValidationException(List<ValidationFailure> failures,
                                      string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
           : this(message, detailCode, payload, innerException)
        {
            var propertyNames = failures
                .Select(e => e.PropertyName)
                .Distinct();

            foreach (var propertyName in propertyNames)
            {
                var propertyFailures = failures
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                Failures.Add(propertyName, propertyFailures);

                var validationInfo = failures.Where(e => e.PropertyName == propertyName)
                                .Select(e => e.FormattedMessagePlaceholderValues)
                                .FirstOrDefault();

                ValidationInfo.Add(propertyName, validationInfo);
            }

            if (failures.Any(f => f.ErrorMessage.Contains("NOT_FOUND") || f.ErrorMessage.Contains("SOME_ITEMS_DELETED")))
            {
                SetDetailCode(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_SOME_ITEMS_DELETED);
            }
        }
    }
}