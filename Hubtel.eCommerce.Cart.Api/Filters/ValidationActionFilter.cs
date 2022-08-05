using System.Net;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hubtel.eCommerce.Cart.Api.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var modelState = context.ModelState;

        //    if (!modelState.IsValid)
        //    {


        //    }
        //}

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                if (badRequestObjectResult.Value is ValidationProblemDetails)
                {
                    context.Result = new BadRequestObjectResult(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = "Invalid JSON input"
                    });
                }
            }
        }
    }
}

