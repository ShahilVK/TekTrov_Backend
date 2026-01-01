using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]    
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
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
}
