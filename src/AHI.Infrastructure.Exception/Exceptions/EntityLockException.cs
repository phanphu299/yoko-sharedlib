using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityLockException : BaseException
    {
        public EntityLockException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_LOCK_LOCKED_BY_OTHER, message, detailCode, payload, innerException)
        {
        }
    }
}