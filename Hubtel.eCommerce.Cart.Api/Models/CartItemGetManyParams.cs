using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItemGetManyParams
    {
        public string PhoneNumber { get; set; } = default;
        public long ProductId { get; set; } = default;
        public int MinQuantity { get; set; } = default;
        public int MaxQuantity { get; set; } = default;
        public DateTime From { get; set; } = default;
        public DateTime To { get; set; } = default;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
