using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface IExceptionHandlerService
    {

        public void LogException(HttpContext httpContext, Exception exception);

        public void LogInvalidRequestInputException(HttpContext httpContext, Exception exception);

        public Task HandleExceptionAsync(HttpContext httpContext, Exception exception);

        public Task HandleInvalidRequestInputExceptionAsync(HttpContext httpContext, Exception exception);
    }
}
