using System.Collections.Generic;

namespace AHI.Infrastructure.Exception.Helper
{
    public class EntityValidationExceptionHelper
    {
        public static EntityValidationException GenerateException(
            string fieldName,
            string fieldErrorMessage,
            string message = null,
            string detailCode = null,
            IDictionary<string, object> payload = null,
            System.Exception innerException = null,
            string fieldValue = null)
        {
            var failure = new FluentValidation.Results.ValidationFailure(fieldName, fieldErrorMessage)
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>{
                    {"propertyName", fieldName},
                    {"propertyValue", fieldValue}
                }
            };

            return GenerateException(
                new List<FluentValidation.Results.ValidationFailure> { failure },
                message,
                detailCode,
                payload,
                innerException);
        }
        public static EntityValidationException GenerateException(
            List<FluentValidation.Results.ValidationFailure> failures,
            string message = null,
            string detailCode = null,
            IDictionary<string, object> payload = null,
            System.Exception innerException = null)
        {
            return new EntityValidationException(
                failures,
                message,
                detailCode,
                payload,
                innerException);
        }
    }
}
