using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/Admin/users")]
[Authorize(Roles = Roles.Admin)]    
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(
                ApiResponse<object>.FailureResponse(
                    "User not found",
                    404
                ));
        }

        return Ok(
            ApiResponse<object>.SuccessResponse(
                user,
                "User fetched successfully"
            ));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(
            ApiResponse<object>.SuccessResponse(
                users,
                "Users fetched successfully"
            )
        );
    }




    //[HttpPatch("{userId:int}/block")]
    //public async Task<IActionResult> BlockUser(int userId)
    //{
    //    await _userService.BlockUserAsync(userId);

    //    return Ok(ApiResponse<bool>.SuccessResponse(
    //        true,
    //        "User blocked successfully"
    //    ));
    //}

    //[HttpPatch("{userId:int}/unblock")]
    //public async Task<IActionResult> UnblockUser(int userId)
    //{
    //    await _userService.UnblockUserAsync(userId);

    //    return Ok(ApiResponse<bool>.SuccessResponse(
    //        true,
    //        "User unblocked successfully"
    //    ));
    //}

    [HttpPatch("{userId:int}/block-toggle")]
    public async Task<IActionResult> ToggleBlockUser(int userId)
    {
        var isBlocked = await _userService.ToggleBlockUserAsync(userId);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            isBlocked ? "User blocked successfully" : "User unblocked successfully"
        ));
    }
}
