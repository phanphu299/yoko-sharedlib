using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class GenericProcessFailedException : BaseException
    {
        public GenericProcessFailedException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_GENERIC_PROCESS_FAILED, message, detailCode, payload, innerException)
        {
        }
    }
}