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
    }
}
