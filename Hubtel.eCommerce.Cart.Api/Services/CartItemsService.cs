#nullable disable
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using System.Threading.Tasks;
using System.Linq;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class CartItemsService : ControllerService, ICartItemsService
    {
        protected readonly CartContext _context;

        public CartItemsService(CartContext context)
        {
            _context = context;
        }

        public void ValidateGetCartItemsQueryString(CartItemGetManyParams queryParams)
        {
            if (queryParams.PhoneNumber != default && (queryParams.PhoneNumber.Length > 15 || queryParams.PhoneNumber.Length < 9))
            {
                throw new InvalidRequestInputException("Invalid phone number");
            }

            if (queryParams.ProductId != default && queryParams.ProductId <= 0)
            {
                throw new InvalidRequestInputException("Product id must be greater than 0");
            }

            if ((queryParams.MinQuantity != default && queryParams.MinQuantity <= 0) || (queryParams.MaxQuantity != default && queryParams.MaxQuantity <= 0))
            {
                throw new InvalidRequestInputException("Any specified item quantity must be greater than 0");
            }

            if (queryParams.From != default && queryParams.To != default && queryParams.From > queryParams.To)
            {
                throw new InvalidRequestInputException("Start date must be less than end date");
            }

            ValidatePaginationQueryString(queryParams.Page, queryParams.PageSize);
        }

        public async Task<Pagination<CartItem>> GetCartItems(CartItemGetManyParams queryParams)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            var items = _context.CartItems.AsNoTracking();

            if (queryParams.PhoneNumber != default)
            {
                items = items.Where(e => e.User.PhoneNumber == queryParams.PhoneNumber);
            }

            if (queryParams.ProductId != default)
            {
                items = items.Where(e => e.ProductId == queryParams.ProductId);
            }

            if (queryParams.MinQuantity != default)
            {
                items = items.Where(e => e.Quantity >= queryParams.MinQuantity);
            }

            if (queryParams.MaxQuantity != default)
            {
                items = items.Where(e => e.Quantity <= queryParams.MaxQuantity);
            }

            if (queryParams.From != default)
            {
                items = items.Where(e => e.Quantity <= queryParams.MaxQuantity);
            }

            if (queryParams.From != default)
            {
                items = items.Where(e => e.CreatedAt >= queryParams.From);
            }

            if (queryParams.To != default)
            {
                items = items.Where(e => e.CreatedAt <= queryParams.To);
            }

            var query = items.Include(item => item.Product)
                .Include(item => item.User)
                .AsQueryable();

            return await PaginationService.Paginate(query, queryParams.Page, queryParams.PageSize);
        }

        public async Task<CartItem> GetSingleCartItem(long id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return await _context.CartItems
                .Include(e => e.Product)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<CartItem> GetSingleCartItem(long productId, long userId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return await _context.CartItems
                .Include(e => e.Product)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ProductId == productId && e.UserId == userId);
        }

        public async Task<bool> UpdateCartItem(long id, CartItemPostDTO cartItem)
        {
            var updatedCartItem = new CartItem
            {
                Id = id,
                Quantity = cartItem.Quantity,
                ProductId = cartItem.ProductId,
                UserId = cartItem.UserId
            };

            _context.Entry(updatedCartItem).State = EntityState.Modified;

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
        }

        public async Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return await _context.CartItems
                .Include(e => e.Product)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId);
        }

        public async Task<CartItem> RetrieveFullCartItem(long productId, long userId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return await _context.CartItems
                .Include(e => e.Product)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ProductId == productId && e.UserId == userId);
        }

        public async Task<CartItem> RetrieveFullCartItem(long id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return await _context.CartItems.FindAsync(id);
        }

        public bool QuantityFarLessThanCurrentCartItemQuantity(CartItemPostDTO cartItem, CartItem fullItem)
        {
            return cartItem.Quantity < 0 && fullItem.Quantity < (-1 * cartItem.Quantity);
        }

        public bool QuantityNegativeOnCreation(CartItemPostDTO cartItem)
        {
            return cartItem.Quantity <= 0;
        }

        public async Task<bool> DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
        }

        public async Task<bool> UpdateCartItemQuantity(CartItem item, int quantity)
        {
            _context.CartItems.Update(item);

            item.Quantity += quantity;

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
        }

        public async Task<CartItem> CreateCartItem(CartItemPostDTO item)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            var newItem = new CartItem
            {
                UserId = item.UserId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
            _context.CartItems.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem;
        }

        public async Task ValidatePostRequestBody(CartItemPostDTO cartItem)
        {
            if (cartItem.Quantity <= 0)
            {
                throw new InvalidRequestInputException("Invalid product quantity.");
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new InvalidRequestInputException("Invalid product.");
            }

            if (cartItem.Quantity > product.QuantityInStock)
            {
                throw new InvalidRequestInputException("Not enough products.");
            }

            var user = await _context.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new InvalidRequestInputException("Invalid user.");
            }
        }

        public bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}

