using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Products;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);

        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task UpdateStockAsync(int productId, int stock);

        Task CreateProductAsync(CreateProductDTO dto, string? imageUrl);
        Task DeleteProductAsync(int productId);

        Task<List<Product>> SearchProductsAsync(string query);

        Task<List<Product>> GetSortedProductsAsync(ProductSortType sortType);

        Task<PagedResult<ProductResponseDTO>> GetProductsPagedAsync(
       int pageNumber,
       int pageSize);




    }
}
