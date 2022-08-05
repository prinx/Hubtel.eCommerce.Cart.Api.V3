using System;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface ICartItemsService : IControllerService
    {
        public void ValidateGetCartItemsQueryString(CartItemGetManyParams queryParams);

        public Task<Pagination<CartItem>> GetCartItems(CartItemGetManyParams queryParams);

        public Task<CartItem> GetSingleCartItem(long id);

        public Task<CartItem> GetSingleCartItem(long productId, long userId);

        public Task<bool> UpdateCartItem(long id, CartItemPostDTO cartItem);

        public Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem);

        public Task<CartItem> RetrieveFullCartItem(long productId, long userId);

        public Task<CartItem> RetrieveFullCartItem(long id);

        public bool QuantityFarLessThanCurrentCartItemQuantity(CartItemPostDTO cartItem, CartItem fullItem);

        public bool QuantityNegativeOnCreation(CartItemPostDTO cartItem);

        public Task<bool> DeleteCartItem(CartItem cartItem);

        public Task<bool> UpdateCartItemQuantity(CartItem item, int quantity);

        public Task<CartItem> CreateCartItem(CartItemPostDTO item);

        public Task ValidatePostRequestBody(CartItemPostDTO cartItem);

        public bool CartItemExists(long id);
    }
}

