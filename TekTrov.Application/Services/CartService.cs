using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task AddToCartAsync(int userId, int productId)
    {
        var cartItem = await _cartRepository.GetAsync(userId, productId);

        if (cartItem != null)
        {
            // ✅ Product already in cart → increase quantity
            cartItem.Quantity += 1;
            await _cartRepository.UpdateAsync(cartItem);
            return;
        }

        var cart = new Cart
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 1
        };

        await _cartRepository.AddAsync(cart);
    }

    public async Task<List<Cart>> GetCartAsync(int userId)
    {
        return await _cartRepository.GetByUserIdAsync(userId);
    }

    public async Task UpdateCartAsync(int userId, int productId, int quantity)
    {
        var cartItem = await _cartRepository.GetAsync(userId, productId);

        if (cartItem == null)
            throw new Exception("Cart item not found");

        if (quantity <= 0)
        {
            await _cartRepository.RemoveAsync(cartItem);
            return;
        }

        cartItem.Quantity = quantity;
        await _cartRepository.UpdateAsync(cartItem);
    }
}
