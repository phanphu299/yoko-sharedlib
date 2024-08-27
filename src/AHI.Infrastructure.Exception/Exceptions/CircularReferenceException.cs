using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class CircularReferenceException : EntityValidationException
    {
        public CircularReferenceException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(message, detailCode, payload, innerException)
        {
            SetErrorCode(ExceptionErrorCode.ENTITY_CIRCULAR_REFERENCE);
        }
    }
}