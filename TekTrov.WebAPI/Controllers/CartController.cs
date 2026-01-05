using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Cart;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "User")]

public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized(ApiResponse<object>.FailureResponse(
                "Invalid or missing token", 401));
        }

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.AddToCartAsync(userId, productId);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Product added to cart"
        ));
    }
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var cartItems = await _cartService.GetCartAsync(userId);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                cartItems,
                "Cart fetched successfully"
            )
        );
    }


    [HttpPut("{productId:int}")]
    public async Task<IActionResult> UpdateCartItem(
        int productId,
        [FromBody] UpdateCartDTO dto)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _cartService.UpdateCartAsync(
            userId, productId, dto.Quantity);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Cart updated successfully"
        ));
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _cartService.RemoveFromCartAsync(userId, productId);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Product removed from cart"
        ));
    }
}
