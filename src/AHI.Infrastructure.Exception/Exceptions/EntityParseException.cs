using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityParseException : BaseException
    {
        public EntityParseException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_PARSE, message, detailCode, payload, innerException)
        {
        }
    }
}