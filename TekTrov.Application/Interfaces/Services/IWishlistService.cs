using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(int userId, int productId);

        Task<List<Wishlist>> GetWishlistAsync(int userId);
    }
}
