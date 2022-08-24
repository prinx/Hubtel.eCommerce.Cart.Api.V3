#nullable disable
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Hubtel.eCommerce.Cart.Api.Filters;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : CustomControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            _productsService.ValidateGetProductsQueryString(page, pageSize);
            var products = await _productsService.GetProducts(page, pageSize);

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = $"{products.Items.Count} product(s) found",
                Data = products
            });
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(long id)
        {
            var product = await _productsService.GetSingleProduct(id);

            if (product == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/Products/{id}: Product not found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Product not found."
                });
            }

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Ok",
                Data = product
            });
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, ProductPostDTO product)
        {
            _productsService.ValidateSentProduct(product);

            string logMessage;

            //if (id != product.Id)
            //{
            //    logMessage = "Invalid Product or Id.";
            //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

            //    return BadRequest(new ApiResponseDTO
            //    {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Success = false,
            //        Message = logMessage,
            //        Data = product
            //    });
            //}

            if (!_productsService.ProductExists(id))
            {
                logMessage = "Product not found.";
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = logMessage,
                    Data = product
                });
            }

            var updated = await _productsService.UpdateProduct(id, product);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: Error while saving updated product to database. Payload: {product}");

                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: Product updated successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Ok",
                Data = product
            });
        }

        // POST: api/Products
        // To protect from overposting attacks,
        // see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostProduct(ProductPostDTO product)
        {
            product.Name.Trim();

            _productsService.ValidateSentProduct(product);

            if (_productsService.ProductExists(product.Name))
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: " +
                    $"Product with Name {product.Name} already exists. ");
                
                return Conflict(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Message = "Product already exists."
                });
            }

            var newProduct = await _productsService.CreateProduct(product);

            //if (!created)
            //{
            //    _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: " +
            //        $"Error while saving new product to database. Paylaod: {product}. ItemToSave: {newProduct}");

            //    var responseData = JsonSerializer.Serialize(new ApiResponseDTO
            //    {
            //        Status = (int)HttpStatusCode.InternalServerError,
            //        Message = "Something went wrong"
            //    });

            //    return new ContentResult
            //    {
            //        Content = responseData,
            //        ContentType = "application/json",
            //        StatusCode = (int)HttpStatusCode.InternalServerError
            //    };
            //}

            _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: " +
                $"Product with name '{product.Name}' created successfully.");

            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "Product created successfully.",
                Data = newProduct
            });
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _productsService.RetrieveProduct(id);

            if (product == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: " +
                    $"Product with id {id} does not exist. Cannot delete product.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Product not found."
                });
            }

            var deleted = await _productsService.DeleteProduct(product);

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: Error while deleting product from database. Payload: {product}");

                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: " +
                $"Product with id {id} deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Ok",
                Data = product
            });
        }
    }
}
