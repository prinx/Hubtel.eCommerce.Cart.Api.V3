#nullable  disable
using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class UsersService : ControllerService, IUsersService
    {
        protected readonly CartContext _context;

        public UsersService(CartContext context)
        {
            _context = context;
        }

        public void ValidateGetUsersQueryString(int page = default, int pageSize = default)
        {
            ValidatePaginationQueryString(page, pageSize);
        }

        public async Task<Pagination<User>> GetUsers(int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            return await PaginationService.Paginate(query, page, pageSize);
        }

        public async Task<User> GetSingleUser(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UpdateUser(long id, UserPostDTO user)
        {
            var updatedUser = new User
            {
                Id = id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            _context.Entry(updatedUser).State = EntityState.Modified;

            var numRowChanged = await _context.SaveChangesAsync();

            return numRowChanged == 1;
        }

        public async Task<User> CreateUser(UserPostDTO user)
        {
            var newUser = new User
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> RetrieveUser(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> DeleteUser(User user)
        {
            _context.Users.Remove(user);

            var numRowChanged = await _context.SaveChangesAsync();

            return numRowChanged == 1;
        }

        public void ValidateSentUser(UserPostDTO user)
        {
            if (user.Name.Length <= 1)
            {
                throw new InvalidRequestInputException("User name too short");
            }

            if (user.Name.Length > 50)
            {
                throw new InvalidRequestInputException("User name too long");
            }

            if (user.PhoneNumber.Length <= 9)
            {
                throw new InvalidRequestInputException("Phone number too short");
            }

            if (user.PhoneNumber.Length > 15)
            {
                throw new InvalidRequestInputException("Phone number too long");
            }
        }

        public bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public bool UserExists(string phoneNumber)
        {
            return _context.Users.Any(e => e.PhoneNumber == phoneNumber);
        }
    }
}

