using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class GenericCommonException : BaseException
    {
        public GenericCommonException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_GENERIC_COMMON_EXCEPTION, message, detailCode, payload, innerException)
        {
        }
    }
}