using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetAsync(int userId, int productId);
        Task AddAsync(Wishlist wishlist);

        Task<List<Wishlist>> GetByUserIdAsync(int userId);
    }
}
