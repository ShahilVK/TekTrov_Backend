using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetAsync(int userId, int productId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);

        Task RemoveAsync(Cart cart);

        Task<List<Cart>> GetByUserIdAsync(int userId);

        Task RemoveRangeAsync(List<Cart> carts);
    }
}
