using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Users;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.User)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("My Profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(
                    "Invalid token", 401)
            );
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound(
                ApiResponse<object>.FailureResponse(
                    "User not found", 404)
            );
        }

        return Ok(
            ApiResponse<object>.SuccessResponse(
                user,
                "Profile fetched successfully"
            )
        );
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(
       [FromBody] ChangePasswordDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<object>.FailureResponse(
                    "Validation failed", 400)
            );
        }

        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _userService.ChangePasswordAsync(userId, dto);

        return Ok(
            ApiResponse<bool>.SuccessResponse(
                true,
                "Password changed successfully"
            )
        );
    }

    //[Authorize]
    //[HttpPatch("My Profile/wishlist")]
    //public async Task<IActionResult> UpdateWishlist(
    //[FromBody] UpdateWishlistDTO dto)
    //{
    //    var userId = int.Parse(
    //        User.FindFirst(ClaimTypes.NameIdentifier)!.Value
    //    );

    //    await _userService.UpdateWishlistAsync(userId, dto.ProductIds);

    //    return Ok(ApiResponse<bool>.SuccessResponse(
    //        true,
    //        "Wishlist updated successfully"
    //    ));
    //}

}
