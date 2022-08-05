using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        [NonAction]
        public ActionResult InternalServerError(object payload)
        {
            var responseData = JsonSerializer.Serialize(payload);

            return new ContentResult
            {
                Content = responseData,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
