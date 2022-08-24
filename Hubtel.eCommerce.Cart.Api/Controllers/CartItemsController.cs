#nullable disable
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Hubtel.eCommerce.Cart.Api.Filters;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        protected readonly ICartItemsService _cartItemsService;
        protected readonly ILogger<CartItemsController> _logger;

        public CartItemsController(ICartItemsService cartItemsService, ILogger<CartItemsController> logger)
        {
            _logger = logger;
            _cartItemsService = cartItemsService;
        }

        // GET: api/v1/CartItems
        [HttpGet]
        public async Task<ActionResult> GetCartItems(CartItemGetManyParams queryParams)
        {
            _cartItemsService.ValidateGetCartItemsQueryString(queryParams);

            var pageItems = await _cartItemsService.GetCartItems(queryParams);

            var message = $"{pageItems.Items.Count} cart item(s) found.";
                
            _logger.LogInformation($"[{DateTime.Now}] GET: api/v1/CartItems: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = pageItems
            });
        }

        // GET: api/v1/CartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCartItem(long id)
        {
            string message;
            var item = await _cartItemsService.GetSingleCartItem(id);

            if (item == null)
            {
                message = "Cart item not found.";
                _logger.LogInformation($"[{DateTime.Now}] GET: api/v1/CartItems/{id}: {message}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = message
                });
            }

            message = "Ok";
            _logger.LogInformation($"[{DateTime.Now}] GET: api/v1/CartItems/{id}: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = item
            });
        }

        // PUT: api/v1/CartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(long id, CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);

            //if (id != cartItem.Id)
            //{
            //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/CartItems/{id}: Id and cart item to update mismatch.");

            //    return BadRequest(new ApiResponseDTO
            //    {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Message = "Id and cart item to update mismatch."
            //    });
            //}

            if (!_cartItemsService.CartItemExists(id))
            {
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/CartItems/{id}: Cart item to update not found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Cart item to update not found."
                });
            }

            var updated = await _cartItemsService.UpdateCartItem(id, cartItem);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products: Error while saving updated cart to database. Payload: {cartItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong."
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/CartItems/{id}: Cart item updated successfuly.");

            return NoContent();
        }

        // POST: api/v1/CartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCartItem(CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);

            CartItem fullItem = await _cartItemsService.RetrieveFullCartItem(cartItem);

            if (fullItem == null)
            {
                var newItem = await _cartItemsService.CreateCartItem(cartItem);

                _logger.LogInformation($"[{DateTime.Now}] POST: api/v1/CartItems: New cart item created for user {cartItem.UserId}");
                newItem.User = null;

                return CreatedAtAction(nameof(GetCartItem), new { id = newItem.Id }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Product(s) added to cart successfully.",
                    Data = newItem
                });
            }

            fullItem.User = null;
            var updated = await _cartItemsService.UpdateCartItemQuantity(fullItem, cartItem.Quantity);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: Error while saving updated cart to database. Payload: {cartItem} Item: {fullItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong."
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] POST: api/v1/CartItems: Product {fullItem.Product.Name} quantity increased in the cart of user {cartItem.UserId}");

            return CreatedAtAction(nameof(GetCartItem), new { id = fullItem.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "Product(s) added to cart successfully.",
                Data = fullItem
            });
        }

        // DELETE: api/v1/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(long id)
        {
            var cartItem = await _cartItemsService.RetrieveFullCartItem(id);

            if (cartItem == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/CartItems/{id}: Cart item does not exist. Cannot delete.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found."
                });
            }

            var deleted = await _cartItemsService.DeleteCartItem(cartItem);

            cartItem.User = null;

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/CartItems/{id}: Error while deleting cart from database. Payload: {cartItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong."
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/CartItems/{id}: Cart deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "cart item deleted successfully."
            });
        }
    }
}
