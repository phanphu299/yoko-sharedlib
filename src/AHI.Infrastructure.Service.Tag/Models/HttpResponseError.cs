using System.Collections.Generic;
using System.Linq;
using AHI.Infrastructure.Exception;
using static AHI.Infrastructure.Exception.Model.ValidationResultApiResponse;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public class HttpResponseError
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public string DetailCode { get; set; }
        public IEnumerable<FieldFailureMessage> Fields { get; set; }

        public System.Exception GenerateException(string message)
        {
            if (ErrorCode == ExceptionErrorCode.ERROR_ENTITY_VALIDATION)
            {
                var lstFailures = Fields.Select(f => new FluentValidation.Results.ValidationFailure(f.Name, f.ErrorCode)).ToList();
                return new EntityValidationException(lstFailures, message: Message, detailCode: DetailCode);
            }
            return new SystemCallServiceException(message ?? Message);
        }
    }
}