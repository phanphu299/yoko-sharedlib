using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Constant;

namespace AHI.Infrastructure.SharedKernel.Abstraction
{
    public interface IErrorService
    {
        bool HasError { get; }

        void RegisterError(string message, ErrorType errorType = ErrorType.UNDEFINED, IDictionary<string, object> validationInfo = null);
    }
}
