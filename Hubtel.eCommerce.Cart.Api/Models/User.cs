using System;
using System.Collections.Generic;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public List<CartItem> CartItems { get; set; }
    }
}
