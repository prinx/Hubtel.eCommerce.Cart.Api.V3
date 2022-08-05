using System;

namespace Hubtel.eCommerce.Cart.Api.Exceptions
{
    [Serializable]
    public class InvalidRequestInputException : ArgumentException
    {
        public InvalidRequestInputException(string message) : base(message)
        {
        }
    }
}

