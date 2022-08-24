using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartDTO
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int QuantityInStock { get; set; }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
