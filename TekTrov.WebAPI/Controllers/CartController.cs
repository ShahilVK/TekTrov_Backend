using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TekTrov.Application.Common;
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
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(
                    "Invalid or missing token",
                    401
                ));
        }

        //var userId = int.Parse(userIdClaim.Value);
        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);


        await _cartService.AddToCartAsync(userId, productId);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                null,
                "Product added to cart"
            ));
    }

    // ✅ GET /api/cart
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(
                    "Invalid or missing token",
                    401
                ));
        }

        var userId = int.Parse(userIdClaim.Value);

        var cartItems = await _cartService.GetCartAsync(userId);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                cartItems,
                "Cart items fetched successfully"
            ));
    }

    // ✅ PUT /api/cart/{productId}
    [HttpPut("{productId:int}")]
    public async Task<IActionResult> UpdateCartItem(
        int productId,
        [FromBody] UpdateCartDTO dto)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(
                    "Invalid or missing token",
                    401
                ));
        }

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.UpdateCartAsync(
            userId,
            productId,
            dto.Quantity
        );

        return Ok(
            ApiResponse<object>.SuccessResponse(
                null,
                "Cart updated successfully"
            ));
    }
}
