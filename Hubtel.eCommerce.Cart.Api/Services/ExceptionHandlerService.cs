using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
        private readonly ILogger<ExceptionHandlerService> _logger;

        public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
        {
            _logger = logger;
        }

        public void LogException(HttpContext httpContext, Exception exception)
        {
            var method = httpContext.Request.Method;
            var query = httpContext.Request.Path;

            _logger.LogError($"[{DateTime.Now}] {method} {query}: Something went wrong: {exception}");
        }

        public void LogInvalidRequestInputException(HttpContext httpContext, Exception exception)
        {
            var method = httpContext.Request.Method;
            var query = httpContext.Request.Path;

            _logger.LogError($"[{DateTime.Now}] {method} {query}: {exception.Message}");
        }

        public async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var responseText = JsonSerializer.Serialize(new ApiResponseDTO
            {
                Status = httpContext.Response.StatusCode,
                Message = "Internal Server Error",
            });

            await httpContext.Response.WriteAsync(responseText);
        }

        public async Task HandleInvalidRequestInputExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var responseText = JsonSerializer.Serialize(new ApiResponseDTO
            {
                Status = httpContext.Response.StatusCode,
                Message = exception.Message,
            });

            await httpContext.Response.WriteAsync(responseText);
        } 
    }
}
