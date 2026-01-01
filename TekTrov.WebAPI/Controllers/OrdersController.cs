using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    // ✅ GET /api/orders
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var orders = await _orderService.GetOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var order = await _orderService
            .GetOrderByIdAsync(orderId, userId);

        if (order == null)
            return NotFound("Order not found");

        return Ok(order);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _orderService.PlaceOrderAsync(userId);
        return Ok("Order placed successfully");
    }
}
