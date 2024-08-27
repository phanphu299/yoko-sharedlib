using System.Collections.Generic;

namespace AHI.Infrastructure.Exception
{
    public class BaseException : System.Exception
    {
        public string ErrorCode { get; private set; }
        public string DetailCode { get; private set; }
        public IDictionary<string, object> Payload { get; private set; }
        public BaseException(
            string errorCode,
            string message,
            string detailCode,
            IDictionary<string, object> payload,
            System.Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            DetailCode = !string.IsNullOrEmpty(detailCode) ? detailCode : errorCode;
            Payload = payload ?? new Dictionary<string, object>();
        }

        internal void SetErrorCode(string errorCode)
        {
            ErrorCode = errorCode;
        }

        internal void SetDetailCode(string detailCode)
        {
            DetailCode = detailCode;
        }
    }
}