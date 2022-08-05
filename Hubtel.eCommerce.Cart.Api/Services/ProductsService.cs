#nullable  disable
using System;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class ProductsService : ControllerService, IProductsService
    {
        protected readonly CartContext _context;

        
        public ProductsService(CartContext context)
        {
            _context = context;
        }

        public void ValidateGetProductsQueryString(int page = default, int pageSize = default)
        {
            ValidatePaginationQueryString(page, pageSize);
        }

        public async Task<Pagination<Product>> GetProducts(int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();
            return await PaginationService.Paginate(query, page, pageSize);
        }

        public async Task<Product> GetSingleProduct(long id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> UpdateProduct(long id, ProductPostDTO product)
        {
            var updatedProduct = new Product
            {
                Id = id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                QuantityInStock = product.QuantityInStock
            };

            _context.Entry(updatedProduct).State = EntityState.Modified;

            var numRowChanged = await _context.SaveChangesAsync();

            return numRowChanged == 1;
        }

        public async Task<Product> CreateProduct(ProductPostDTO product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                QuantityInStock = product.QuantityInStock
            };

            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            return newProduct;
        }

        public async Task<Product> RetrieveProduct(long id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            _context.Products.Remove(product);

            var numRowChanged = await _context.SaveChangesAsync();

            return numRowChanged == 1;
        }

        public void ValidateSentProduct(ProductPostDTO product)
        {
            if (product.UnitPrice < 0)
            {
                throw new InvalidRequestInputException("Product unit price invalid");
            }

            if (product.Name.Length <= 1)
            {
                throw new InvalidRequestInputException("Product name too short");
            }

            if (product.Name.Length > 50)
            {
                throw new InvalidRequestInputException("Product name too long");
            }
        }

        public bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public bool ProductExists(string name)
        {
            return _context.Products.Any(e => e.Name == name);
        }
    }
}

