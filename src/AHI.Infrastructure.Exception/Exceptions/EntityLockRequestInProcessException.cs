using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityLockRequestInProcessException : BaseException
    {
        public EntityLockRequestInProcessException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_LOCK_REQUESTED_IN_PROCESS, message, detailCode, payload, innerException)
        {
        }
    }
}