using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class SystemNotSupportedException : BaseException
    {
        public SystemNotSupportedException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_SYSTEM_NOT_SUPPORTED, message, detailCode, payload, innerException)
        {
        }
    }
}