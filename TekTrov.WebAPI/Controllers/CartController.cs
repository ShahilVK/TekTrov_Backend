using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TekTrov.Application.DTOs.Cart;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // ✅ POST /api/cart/{productId}
    [HttpPost("{productId:int}")]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _cartService.AddToCartAsync(userId, productId);
        return Ok("Product added to cart");
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var cartItems = await _cartService.GetCartAsync(userId);
        return Ok(cartItems);
    }

    [HttpPut("{productId:int}")]
    public async Task<IActionResult> UpdateCartItem(
       int productId,
       UpdateCartDTO dto)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _cartService.UpdateCartAsync(
            userId, productId, dto.Quantity);

        return Ok("Cart updated successfully");
    }
}
