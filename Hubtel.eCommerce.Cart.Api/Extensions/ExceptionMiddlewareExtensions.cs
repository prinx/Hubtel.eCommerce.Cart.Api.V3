using Hubtel.eCommerce.Cart.Api.Middlewares;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace Hubtel.eCommerce.Cart.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IExceptionHandlerService exceptionService)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async httpContext =>
                {
                   var httpContextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

                    if (httpContextFeature != null)
                    {
                        exceptionService.LogException(httpContext, httpContextFeature.Error);
                        await exceptionService.HandleExceptionAsync(httpContext, httpContextFeature.Error);
                    }
                });
            });
        }

        public static void ConfigureCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
