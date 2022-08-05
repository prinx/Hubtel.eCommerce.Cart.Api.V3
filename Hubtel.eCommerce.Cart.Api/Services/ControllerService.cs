using System;
using Hubtel.eCommerce.Cart.Api.Exceptions;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class ControllerService : IControllerService
    {
        public void ValidatePaginationQueryString(int page = default, int pageSize = default)
        {
            if (page <= 0)
            {
                throw new InvalidRequestInputException("Invalid page");
            }

            if (pageSize <= 0)
            {
                throw new InvalidRequestInputException("Invalid page size");
            }

            if (pageSize > 1000)
            {
                throw new InvalidRequestInputException("Page size too big");
            }
        }
    }
}

