using GZone.Service.BusinessModels.Generic;
using GZone.Service.Extensions.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while processing {context.Request.Path}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Mặc định là lỗi 500 Internal Server Error
            int statusCode = (int)HttpStatusCode.InternalServerError;
            //string message = "Internal Server Error";
            string message = ex.Message;    //Suggest using for development environment only

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

                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Unauthorized";
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

            var responseModel = ApiResponse<object>.Failure(message, statusCode);

            context.Response.StatusCode = statusCode;

            // Serialize ApiResponse thành JSON
            var jsonResponse = JsonConvert.SerializeObject(responseModel);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
