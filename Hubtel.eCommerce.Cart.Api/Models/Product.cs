using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal UnitPrice { get; set; } = default!;
        public int QuantityInStock { get; set; } = default!;
        public DateTime CreatedAt { get; set; }

    }
}
