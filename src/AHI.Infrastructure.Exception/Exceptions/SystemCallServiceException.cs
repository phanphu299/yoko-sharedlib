using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class SystemCallServiceException : BaseException
    {
        public SystemCallServiceException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE, message, detailCode, payload, innerException)
        {
        }
    }
}