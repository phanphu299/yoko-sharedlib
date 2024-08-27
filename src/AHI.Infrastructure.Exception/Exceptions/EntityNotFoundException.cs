using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_NOT_FOUND, message, detailCode, payload, innerException)
        {
        }
    }
}