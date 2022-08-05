using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    interface IPagination<T>
    {
        public Task<Pagination<T>> Paginate(IQueryable<T> query, int currentPage, int pageSize);
    }
}
