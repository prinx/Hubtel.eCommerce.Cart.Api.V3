using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    [NotMapped]
    public class Pagination<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }

    }
}
