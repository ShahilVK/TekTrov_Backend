

using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Products;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _productRepository.GetByCategoryAsync(category);
        }

        public async Task CreateProductAsync(
            CreateProductDTO dto,
            string? imageUrl)
        {
            if (dto.Name.StartsWith(" ") || dto.Category.StartsWith(" "))
                throw new Exception("Leading spaces are not allowed");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category.ToLower(),
                Stock = dto.Stock,
                ImageUrl = imageUrl
            };

            await _productRepository.AddAsync(product);
        }


        public async Task UpdateStockAsync(int productId, int stock)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            product.Stock = stock;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            await _productRepository.DeleteAsync(product);
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            return await _productRepository.SearchByNameAsync(
                query.Trim()
            );
        }

        public async Task<List<Product>> GetSortedProductsAsync(ProductSortType sortType)
        {
            return await _productRepository.GetSortedAsync(sortType);
        }

        public async Task<PagedResult<ProductResponseDTO>> GetProductsPagedAsync(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = await _productRepository.CountAsync();
            var products = await _productRepository
                .GetPagedAsync(pageNumber, pageSize);

            return new PagedResult<ProductResponseDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category,
                    ImageUrl = p.ImageUrl
                }).ToList()
            };
        }

    }
}
