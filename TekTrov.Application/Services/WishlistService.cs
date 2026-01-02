using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Wishlist;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task AddToWishlistAsync(int userId, int productId)
        {
            var existing = await _wishlistRepository
                .GetAsync(userId, productId);

            if (existing != null)
                throw new Exception("Product already in wishlist");

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddAsync(wishlist);
        }

        public async Task<List<WishlistItemResponseDTO>> GetWishlistAsync(int userId)
        {
            var wishlists = await _wishlistRepository.GetByUserIdAsync(userId);

            return wishlists.Select(w => new WishlistItemResponseDTO
            {
                ProductId = w.ProductId,
                ProductName = w.Product!.Name,
                Price = w.Product.Price,
                ImageUrl = w.Product.ImageUrl
            }).ToList();
        }
    }
}
