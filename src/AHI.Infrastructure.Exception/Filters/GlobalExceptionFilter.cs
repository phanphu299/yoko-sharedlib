
using System.Diagnostics;
using AHI.Infrastructure.Exception.Model;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AHI.Infrastructure.Exception.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILoggerAdapter<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILoggerAdapter<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            context.ExceptionHandled = true;
            _logger.LogError(exception, exception.Message);
            string traceId = Activity.Current.Context.TraceId.ToString();
            switch (exception)
            {
                case EntityValidationException validation:
                    context.Result = new BadRequestObjectResult(new ValidationResultApiResponse(false, validation.ErrorCode, validation.DetailCode, validation.Payload, validation.Failures, traceId));
                    context.ExceptionHandled = true;
                    break;
                case EntityNotFoundException notFoundValidation:
                    context.Result = new NotFoundObjectResult(new { IsSuccess = false, Message = notFoundValidation.Message, ErrorCode = notFoundValidation.ErrorCode, DetailCode = notFoundValidation.DetailCode, TraceId = traceId, Payload = notFoundValidation.Payload });
                    context.ExceptionHandled = true;
                    break;
                case BaseException baseException:
                    context.Result = new BadRequestObjectResult(new { IsSuccess = false, Message = baseException.Message, ErrorCode = baseException.ErrorCode, DetailCode = baseException.DetailCode, TraceId = traceId });
                    context.ExceptionHandled = true;
                    break;
                default:
                    context.Result = new BadRequestObjectResult(new { IsSuccess = false, ErrorCode = ExceptionErrorCode.ERROR_GENERIC_COMMON_EXCEPTION, TraceId = traceId });
                    break;
            }
        }
    }
}
