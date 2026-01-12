using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Order;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;

namespace TekTrov.WebApi.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = Roles.Admin)]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersForAdminAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                orders,
                "All orders fetched successfully"
            ));
        }

        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
           int orderId,
           [FromBody] UpdateOrderStatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Invalid status", 400));

            await _orderService.UpdateOrderStatusAsync(
                orderId, dto.Status);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Order status updated successfully"
            ));
        }
    }
}
