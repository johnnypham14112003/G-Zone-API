using GZone.Service.BusinessModels.Generic;
using GZone.Service.Extensions.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace GZone.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public enum ErrorType
        {
            NotFound,
            BadRequest,
            Unauthorized,
            Conflict,
            InternalServerError
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (IsAuthError(context.Response.StatusCode) && !context.Response.HasStarted)
                {
                    await HandleAuthErrorAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while processing {context.Request.Path}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static bool IsAuthError(int statusCode)
        {
            return statusCode == StatusCodes.Status401Unauthorized ||
                   statusCode == StatusCodes.Status403Forbidden;
        }

        private async Task HandleAuthErrorAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string message = "You don't have permission";
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                message = "Please check again your authorize token (Unauthorized).";
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                message = "You don't have permission to do this action (Forbidden).";
            }

            var responseModel = ApiResponse<object>.Failure(message, context.Response.StatusCode);
            var jsonResponse = JsonConvert.SerializeObject(responseModel);

            await context.Response.WriteAsync(jsonResponse);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Nếu response đã bắt đầu gửi thì không thể ghi đè
            //if (context.Response.HasStarted) return;

            context.Response.ContentType = "application/json";

            // Mặc định là lỗi 500 Internal Server Error
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";

            // Mapping Exception sang StatusCode
            switch (ex)
            {
                case BadRequestException badRequestEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = badRequestEx.Message; // Lấy message từ Service throw ra
                    break;

                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;

                case UnauthorizedException unAuthorizeEx:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = unAuthorizeEx.Message;
                    break;

                case ConflictException conflictEx:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = conflictEx.Message;
                    break;

                // Xử lý mặc định cho Exception thường nhưng muốn hiện message
                default:
                    message = ex.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            // Serialize ApiResponse thành JSON
            var responseModel = ApiResponse<object>.Failure(message, statusCode);
            var jsonResponse = JsonConvert.SerializeObject(responseModel);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
