using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityInvalidException : BaseException
    {
        public EntityInvalidException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_INVALID, message, detailCode, payload, innerException)
        {
        }
    }
}