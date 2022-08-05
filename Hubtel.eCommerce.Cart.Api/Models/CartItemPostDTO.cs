namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItemPostDTO
    {
        public int Quantity { get; set; }

        public long ProductId { get; set; }

        public long UserId { get; set; }
    }
}
