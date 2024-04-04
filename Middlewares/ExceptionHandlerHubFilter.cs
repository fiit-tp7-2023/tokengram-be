using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Tokengram.Constants;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Models.Validation;
using Tokengram.Utils;

namespace Tokengram.Middlewares
{
    public class ExceptionHandlerHubFilter : IHubFilter
    {
        private readonly ILogger<ExceptionHandlerHubFilter> _logger;

        public ExceptionHandlerHubFilter(ILogger<ExceptionHandlerHubFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(ex);
            }
        }

        private ErrorResponseDTO HandleExceptionAsync(Exception exception)
        {
            var errorResponse = new ErrorResponseDTO();

            switch (exception)
            {
                case HubException ex:
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    break;

                case ValidationException ex:
                    errorResponse.StatusCode = HttpStatusCode.UnprocessableEntity;
                    errorResponse.Message = ErrorMessages.VALIDATION_ERROR;
                    errorResponse.Errors = ErrorUtil.BuildValidationErrors(ex);
                    break;

                default:
                    errorResponse.StatusCode = HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = HttpStatusCode.InternalServerError;
                    errorResponse.Message = ErrorMessages.INTERNAL_SERVER_ERROR;
                    break;
            }

            _logger.LogError(exception, exception.Message);

            return errorResponse;
        }
    }
}
