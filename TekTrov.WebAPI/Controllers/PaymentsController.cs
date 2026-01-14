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

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

     



        [HttpPost("razorpay/payment")]
        public async Task<IActionResult> RazorpayPayment(
    [FromBody] RazorpayPaymentDTO dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _paymentService
                .HandleRazorpayPaymentAsync(userId, dto);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Razorpay payment processed"
            ));
        }

    }
}
