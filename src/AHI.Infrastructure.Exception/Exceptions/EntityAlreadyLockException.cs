using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityAlreadyLockException : BaseException
    {
        public EntityAlreadyLockException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_LOCK_ALREADY_LOCKED, message, detailCode, payload, innerException)
        {
        }
    }
}