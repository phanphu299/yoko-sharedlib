using System.Collections.Generic;
using System.Linq;

namespace AHI.Infrastructure.Exception.Model
{
    public class ValidationResultApiResponse
    {
        public bool IsSuccess { get; }

        /// <summary>
        /// Validation result message.
        /// </summary>
        public string Message { get; set; } = ExceptionErrorCode.DetailCode.ERROR_VALIDATION;
        public string ErrorCode { get; set; }
        public string DetailCode { get; set; }
        public string TraceId { get; set; }
        /// <summary>
        /// Property to its validation failures.
        /// </summary>
        public ICollection<FieldFailureMessage> Fields { get; } = new List<FieldFailureMessage>();

        public ValidationResultApiResponse(bool isSuccess, string errorCode, string detailCode, string traceId)
        {
            Fields = new LinkedList<FieldFailureMessage>();
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            DetailCode = detailCode;
            TraceId = traceId;
        }

        public ValidationResultApiResponse(bool isSuccess, string errorCode, string detailCode, IDictionary<string, object> payload, IDictionary<string, string[]> failures, string traceId) : this(isSuccess, errorCode, detailCode, traceId)
        {
            if (failures == null || !failures.Any())
                return;

            Fields = failures.SelectMany(fieldFailures => fieldFailures.Value.Select(item => new FieldFailureMessage
            {
                Name = fieldFailures.Key,
                ErrorCode = item,
                ErrorDetail = payload
            })).ToList();
        }

        public class FieldFailureMessage
        {
            public string Name { get; set; }
            public string ErrorCode { get; set; }
            public IDictionary<string, object> ErrorDetail { get; set; } = new Dictionary<string, object>();
        }
    }
}
