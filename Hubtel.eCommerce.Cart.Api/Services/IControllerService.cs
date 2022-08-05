using System;
namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface IControllerService
    {
        public void ValidatePaginationQueryString(int page = default, int pageSize = default);
    }
}

