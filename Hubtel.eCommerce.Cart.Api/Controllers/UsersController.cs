#nullable disable
using Microsoft.AspNetCore.Mvc;
using Hubtel.eCommerce.Cart.Api.Models;
using System.Net;
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
    public class UsersController : CustomControllerBase
    {
        protected readonly IUsersService _usersService;
        protected readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        // GET: api/v1/Users
        [HttpGet]
        public async Task<ActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            _usersService.ValidateGetUsersQueryString(page, pageSize);
            var users = await _usersService.GetUsers(page, pageSize);

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = $"{users.Items.Count} user(s) found.",
                Data = users
            });
        }

        // GET: api/v1/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(long id)
        {
            var user = await _usersService.GetSingleUser(id);

            if (user == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/v1/Users/{id}: User not found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Ok",
                Data = user
            });
        }

        // PUT: api/v1/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserPostDTO user)
        {
            _usersService.ValidateSentUser(user);

            string logMessage;

            //if (id != user.Id)
            //{
            //    logMessage = "Invalid User or Id.";
            //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/Users/{id}: {logMessage}");

            //    return BadRequest(new ApiResponseDTO
            //   {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Message = logMessage,
            //        Data = user
            //    });
            //}

            if (!_usersService.UserExists(id))
            {
                logMessage = "User not found.";
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/Users/{id}: {logMessage}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = logMessage,
                    Data = user
                });
            }

            var updated = await _usersService.UpdateUser(id, user);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/Users/{id}: Error while saving updated user to database. Payload: {user}");

                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong."
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] PUT: api/v1/Users/{id}: User updated successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "User updated successfully.",
                Data = user
            });
        }

        // POST: api/v1/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostUser(UserPostDTO user)
        {
            user.Name.Trim();
            user.PhoneNumber.Trim();

            _usersService.ValidateSentUser(user);

            if (_usersService.UserExists(user.PhoneNumber))
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/v1/Users: " +
                    $"User with phone number {user.PhoneNumber} already exists.");

                return Conflict(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Message = "User already exists.",
                    Data = user
                });
            }

            var newUser = await _usersService.CreateUser(user);

            _logger.LogInformation($"[{DateTime.Now}] POST: api/v1/Users: User with phone number {user.PhoneNumber} created successfully.");

            return CreatedAtAction("GetUser", new { id = newUser.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "User created successfully.",
                Data = newUser
            });
        }

        // DELETE: api/v1/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _usersService.RetrieveUser(id);

            if (user == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/Users/{id}: User does not exist. Cannot delete.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                });
            }

            var deleted = await _usersService.DeleteUser(user);

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/Users/{id}: Error while deleting user from database. Payload: {user}");

                return InternalServerError(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong."
                });
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/v1/Users/{id}: User deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "User deleted successfully."
            });
        }
    }
}
