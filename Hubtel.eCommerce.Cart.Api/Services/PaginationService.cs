using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public static class PaginationService
    {
        private static readonly int _defaultPageSize = 3;

        public static async Task<Pagination<T>> Paginate<T>(IQueryable<T> query, int currentPage, int pageSize = 0)
        {
            pageSize = pageSize == 0 ? _defaultPageSize : pageSize;

            Pagination <T> pagination = new Pagination<T>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = query.Count()
            };

            int skip = (currentPage - 1) * pageSize;

            pagination.Items = await query.Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return pagination;
        }
    }
}
