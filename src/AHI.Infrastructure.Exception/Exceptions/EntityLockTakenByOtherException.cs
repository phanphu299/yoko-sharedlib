using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class EntityLockTakenByOtherException : BaseException
    {
        public EntityLockTakenByOtherException(string message = null,
                                      string detailCode = null,
                                      IDictionary<string, object> payload = null,
                                      System.Exception innerException = null)
            : base(ExceptionErrorCode.ERROR_ENTITY_LOCK_REQUESTED_TAKEN_LOCK_BY_OTHER, message, detailCode, payload, innerException)
        {
        }
    }
}