using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class SystemSecurityException : BaseException
    {
        public SystemSecurityException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_SYSTEM_SECURITY_EXCEPTION, message, detailCode, payload, innerException)
        {
        }
    }
}