using System.Net;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Constants;
using Tokengram.Models.Exceptions;
using FluentValidation;
using Newtonsoft.Json;
using Tokengram.Utils;

namespace Tokengram.Infrastructure.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            var errorResponse = new ErrorResponseDTO();

            switch (exception)
            {
                // System exceptions
                case UnauthorizedAccessException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.StatusCode = HttpStatusCode.Unauthorized;
                    errorResponse.Message = ErrorMessages.UNAUTHORIZED;
                    break;

                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        errorResponse.StatusCode = HttpStatusCode.Forbidden;
                        errorResponse.Message = ErrorMessages.ACCESS_TOKEN_INVALID;
                        break;
                    }

                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    break;

                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;

                case ValidationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = ErrorMessages.VALIDATION_ERROR;
                    errorResponse.Errors = ErrorUtil.BuildValidationErrors(ex);
                    break;

                // Custom exceptions
                case BadRequestException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    break;

                case UnauthorizedException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.StatusCode = HttpStatusCode.Unauthorized;
                    errorResponse.Message = ex.Message;
                    break;

                case ForbiddenException ex:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.StatusCode = HttpStatusCode.Forbidden;
                    errorResponse.Message = ex.Message;
                    break;

                case NotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = HttpStatusCode.InternalServerError;
                    errorResponse.Message = ErrorMessages.INTERNAL_SERVER_ERROR;
                    break;
            }

            _logger.LogError(exception, exception.Message);
            var result = JsonConvert.SerializeObject(
                errorResponse,
                new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
            );
            await context.Response.WriteAsync(result);
        }
    }
}
