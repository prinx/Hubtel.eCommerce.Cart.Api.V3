#nullable disable
using Microsoft.AspNetCore.Mvc;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using System.Net;
using System.Text.Json;
using Hubtel.eCommerce.Cart.Api.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartsController : CustomControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly ILogger<CartItemsController> _logger;

        public CartsController(ICartItemsService cartItemsService, ILogger<CartItemsController> logger)
        {
            _logger = logger;
            _cartItemsService = cartItemsService;
        }

        // 1. Provide an endpoint to Add items to cart, with specified quantity
        // Adding similar items(same item ID) should increase the quantity - POST

        // POST: api/Carts
        [HttpPost]
        public async Task<ActionResult> PostCartItem(CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);

            CartItem fullItem = await _cartItemsService.RetrieveFullCartItem(cartItem);

            if (fullItem == null)
            {
                var newItem = await _cartItemsService.CreateCartItem(cartItem);

                _logger.LogInformation($"[{DateTime.Now}] POST: api/v1/Carts: New cart item created for user {cartItem.UserId}");

                return CreatedAtAction(nameof(GetSingleCartItem), new { productId = cartItem.ProductId, userId = cartItem.UserId }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Item(s) added to cart successfully",
                    Data = newItem
                });
            }

            var updated = await _cartItemsService.UpdateCartItemQuantity(fullItem, cartItem.Quantity);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Carts: Error while saving updated cart to database. Payload: {cartItem} Item: {fullItem}");

                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] POST: api/Carts: Product {fullItem.Product.Name} quantity increased in the cart of user {cartItem.UserId}");

            return CreatedAtAction(nameof(GetSingleCartItem), new { productId = cartItem.ProductId, userId = cartItem.UserId }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "Product(s) added to cart successfully.",
                Data = fullItem
            });
        }

        // 2.a. Provide an endpoint to remove an item from cart - DELETE verb
        // DELETE: api/Carts/5/2
        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> DeleteCartItem(long userId, long productId)
        {
            var cartItem = await _cartItemsService.RetrieveFullCartItem(productId, userId);

            if (cartItem == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{userId}/{productId}: Cart item does not exist.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found."
                });
            }

            var deleted = await _cartItemsService.DeleteCartItem(cartItem);

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{userId}/{productId}: Error while deleting cart item from database.");
                
                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{userId}/{productId}: Cart item deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Cart item deleted sucessfully"
            });
        }

        // 2.b. Provide an endpoint to remove an item from cart - DELETE verb
        // DELETE: api/Carts/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCartItem(long id)
        //{
        //    var cartItem = await _cartItemsService.RetrieveFullCartItem(id);

        //    if (cartItem == null)
        //    {
        //        _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{id}: Cart item does not exist. Cannot delete.");

        //        return NotFound(new ApiResponseDTO
        //        {
        //            Status = (int)HttpStatusCode.NotFound,
        //            Message = "Cart item not found."
        //        });
        //    }

        //    var deleted = await _cartItemsService.DeleteCartItem(cartItem);

        //    cartItem.User = null;

        //    if (!deleted)
        //    {
        //        _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{id}: Error while deleting cart from database. Payload: {cartItem}");

        //        return InternalServerError(new ApiResponseDTO
        //        {
        //            Status = (int)HttpStatusCode.InternalServerError,
        //            Message = "Something went wrong"
        //        });
        //    }

        //    _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Carts/{id}: Cart deleted successfully.");

        //    return Ok(new ApiResponseDTO
        //    {
        //        Status = (int)HttpStatusCode.OK,
        //        Success = true,
        //        Message = "Cart item deleted sucessfully"
        //    });
        //}

        // 3. Provide an endpoint list all cart items (with filters => phoneNumbers, time, quantity, item - GET
        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult> GetCartItems([FromQuery] CartItemGetManyParams queryParams)
        {
            _cartItemsService.ValidateGetCartItemsQueryString(queryParams);

            var pageItems = await _cartItemsService.GetCartItems(queryParams);

            var message = $"{pageItems.Items.Count} cart item(s) found.";

            _logger.LogInformation($"[{DateTime.Now}] GET: api/Carts: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = pageItems
            });
        }

        // 4.a. Provide endpoint to get single item - GET
        // GET: api/Carts/4/5
        [HttpGet("{userId}/{productId}")]
        public async Task<ActionResult> GetSingleCartItem(long userId, long productId)
        {
            string message;
            var item = await _cartItemsService.GetSingleCartItem(productId, userId);

            if (item == null)
            {
                message = "Cart item not found.";
                _logger.LogInformation($"[{DateTime.Now}] GET: api/Carts/{userId}/{productId}: {message}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = message
                });
            }

            message = "Found.";
            _logger.LogInformation($"[{DateTime.Now}] GET: api/Carts/{userId}/{productId}: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = item
            });
        }

        // 4.b. Provide endpoint to get single item - GET
        // GET: api/Carts/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult> GetCartItem(long id)
        //{
        //    string message;
        //    var item = await _cartItemsService.GetSingleCartItem(id);

        //    if (item == null)
        //    {
        //        message = "Cart item not found.";
        //        _logger.LogInformation($"[{DateTime.Now}] GET: api/Carts/{id}: {message}");

        //        return NotFound(new ApiResponseDTO
        //        {
        //            Status = (int)HttpStatusCode.NotFound,
        //            Message = message
        //        });
        //    }

        //    message = "Found.";
        //    _logger.LogInformation($"[{DateTime.Now}] GET: api/Carts/{id}: {message}");

        //    return Ok(new ApiResponseDTO
        //    {
        //        Status = (int)HttpStatusCode.OK,
        //        Success = true,
        //        Message = message,
        //        Data = item
        //    });
        //}
    }
}
