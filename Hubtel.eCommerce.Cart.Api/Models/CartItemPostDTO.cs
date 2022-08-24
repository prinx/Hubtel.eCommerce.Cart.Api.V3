namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItemPostDTO
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public long UserId { get; set; }
    }
}
