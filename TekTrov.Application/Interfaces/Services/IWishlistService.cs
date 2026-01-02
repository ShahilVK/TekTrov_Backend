using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Wishlist;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(int userId, int productId);

        Task<List<WishlistItemResponseDTO>> GetWishlistAsync(int userId);
    }
}
