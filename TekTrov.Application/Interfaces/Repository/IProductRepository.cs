using TekTrov.Application.DTOs.Products;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);

        Task<List<Product>> GetByCategoryAsync(string category);
        Task DeleteAsync(Product product);

        Task<List<Product>> SearchByNameAsync(string query);

        Task<List<Product>> GetSortedAsync(ProductSortType sortType);

        Task<int> CountAsync();
        Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize);




    }
}
