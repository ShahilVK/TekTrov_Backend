using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TekTrov.Application.Common;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _wishlistService.AddToWishlistAsync(userId, productId);
        return Ok("Product added to wishlist");
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var wishlist = await _wishlistService.GetWishlistAsync(userId);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                wishlist,
                "Wishlist fetched successfully"
            )
        );
    }
}
