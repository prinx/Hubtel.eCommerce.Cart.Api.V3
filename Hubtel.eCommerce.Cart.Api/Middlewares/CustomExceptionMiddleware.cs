using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandlerService _exceptionService;

        public CustomExceptionMiddleware(RequestDelegate next, IExceptionHandlerService exceptionService)
        {
            _next = next;
            _exceptionService = exceptionService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (InvalidRequestInputException ex)
            {
                _exceptionService.LogInvalidRequestInputException(httpContext, ex);
                await _exceptionService.HandleInvalidRequestInputExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _exceptionService.LogException(httpContext, ex);
                await _exceptionService.HandleExceptionAsync(httpContext, ex);
            }
        }
    }
}
