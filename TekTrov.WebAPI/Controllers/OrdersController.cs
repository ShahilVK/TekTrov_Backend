using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Order;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    [HttpPost("Direct Order")]
    public async Task<IActionResult> PlaceDirectOrder(
       [FromBody] DirectOrderDTO dto)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _orderService.PlaceDirectOrderAsync(userId, dto);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Order placed successfully"
        ));
    }

    [HttpGet("My-Orders")]
    public async Task<IActionResult> GetOrders()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var orders = await _orderService.GetOrdersAsync(userId);

        return Ok(ApiResponse<object>.SuccessResponse(
            orders,
            "Orders fetched successfully"
        ));
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var order = await _orderService.GetOrderByIdAsync(orderId, userId);
        if (order == null)
            return NotFound(ApiResponse<object>.FailureResponse(
                "Order not found", 404));

        return Ok(ApiResponse<object>.SuccessResponse(
            order,
            "Order fetched successfully"
        ));
    }

    [Authorize(Roles = "User")]
    [HttpPost("Order Inside Cart")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid shipping address", 400));

        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _orderService.PlaceOrderAsync(userId, dto);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Order placed successfully"
        ));
    }


    [HttpPost("{orderId:int}/pay")]
    public async Task<IActionResult> PayOrder(int orderId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _orderService.PayOrderAsync(userId, orderId);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Order payment successful"
        ));
    }

    [Authorize(Roles = "User")]
    [HttpPost("{orderId:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
        );

        await _orderService.CancelOrderAsync(userId, orderId);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Order cancelled successfully"
        ));
    }

}
