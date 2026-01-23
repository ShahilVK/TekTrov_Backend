

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Payments;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentsController(
     IPaymentService paymentService,
     IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpPost("razorpay/create-order")]
        public async Task<IActionResult> CreateRazorpayOrder(
                  [FromBody] CreateRazorpayOrderDTO dto)
        {
            var result = await _paymentService
                .CreateRazorpayOrderAsync(dto.Amount);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Razorpay order created"
            ));
        }

 

        [HttpPost("razorpay/verify")]
        public async Task<IActionResult> VerifyPayment(
    [FromBody] RazorpayPaymentDTO dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            await _paymentService.VerifyPaymentAsync(dto);

            await _orderService.MarkOrderAsPaidAsync(dto.OrderId, userId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Payment verified and stock updated"
            ));
        }

    }
}

