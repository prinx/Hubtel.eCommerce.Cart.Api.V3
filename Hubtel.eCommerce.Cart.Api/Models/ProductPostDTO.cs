namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ProductPostDTO
    {
        public string Name { get; set; } = default!;
        public decimal UnitPrice { get; set; } = default!;
        public int QuantityInStock { get; set; } = default!;
    }
}
