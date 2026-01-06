using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
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

        // ✅ GET ALL ORDERS (ADMIN)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersForAdminAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                orders,
                "All orders fetched successfully"
            ));
        }
    }
}
