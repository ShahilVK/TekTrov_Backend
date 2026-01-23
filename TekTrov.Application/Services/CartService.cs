using TekTrov.Application.DTOs.Cart;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }




    public async Task AddToCartAsync(
    int userId,
    int productId,
    int quantity)
{
    if (quantity <= 0)
        throw new Exception("Invalid quantity");

    var product = await _productRepository.GetByIdAsync(productId)
        ?? throw new Exception("Product not found");

    if (product.Stock < quantity)
        throw new Exception("Insufficient stock");

    var cartItem = await _cartRepository.GetAsync(userId, productId);

    if (cartItem == null)
    {
        await _cartRepository.AddAsync(new Cart
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        });
        return;
    }

    if (cartItem.Quantity + quantity > product.Stock)
        throw new Exception($"Only {product.Stock} items available in stock");

    cartItem.Quantity += quantity;
    await _cartRepository.UpdateAsync(cartItem);
}

    public async Task<List<CartItemResponseDTO>> GetCartAsync(int userId)
    {
        var carts = await _cartRepository.GetByUserIdAsync(userId);

        return carts.Select(c => new CartItemResponseDTO
        {
            ProductId = c.ProductId,
            ProductName = c.Product!.Name,
            Price = c.Product.Price,
            Quantity = c.Quantity,

            ImageUrl = c.Product.ImageUrl
        }).ToList();
    }

    public async Task UpdateCartAsync(int userId, int productId, int quantity)
    {
        var cartItem = await _cartRepository.GetAsync(userId, productId);

        if (cartItem == null)
            throw new Exception("Cart item not found");

        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
            throw new Exception("Product not found");

        if (quantity <= 0)
        {
            await _cartRepository.RemoveAsync(cartItem);
            return;
        }

        if (quantity > product.Stock)
            throw new Exception($"Only {product.Stock} items available in stock");

        cartItem.Quantity = quantity;
        await _cartRepository.UpdateAsync(cartItem);
    }

    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var cartItem = await _cartRepository.GetAsync(userId, productId);

        if (cartItem == null)
            throw new Exception("Cart item not found");

        await _cartRepository.RemoveAsync(cartItem);
    }
}
