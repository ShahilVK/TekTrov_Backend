//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using TekTrov.Application.Common;
//using TekTrov.Application.DTOs.Payments;
//using TekTrov.Application.Interfaces.Services;
//using TekTrov.Application.Services;

//namespace TekTrov.WebApi.Controllers
//{
//    [ApiController]
//    [Route("api/payments")]
//    [Authorize]
//    public class PaymentsController : ControllerBase
//    {
//        private readonly IPaymentService _paymentService;

//        public PaymentsController(IPaymentService paymentService)
//        {
//            _paymentService = paymentService;
//        }





//        [HttpPost("razorpay/payment")]
//        public async Task<IActionResult> RazorpayPayment(
//    [FromBody] RazorpayPaymentDTO dto)
//        {
//            var userId = int.Parse(
//                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
//            );

//            var result = await _paymentService
//                .HandleRazorpayPaymentAsync(userId, dto);

//            return Ok(ApiResponse<object>.SuccessResponse(
//                result,
//                "Razorpay payment processed"
//            ));
//        }




//    }
//}

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

        [HttpPost("razorpay/create-order")]
        public async Task<IActionResult> CreateRazorpayOrder(
            [FromBody] CreateRazorpayOrderDTO dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _paymentService
                .CreateRazorpayOrderAsync(userId, dto.OrderId);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Razorpay order created successfully"
            ));
        }

        // ✅ STEP 2: VERIFY PAYMENT AFTER SUCCESS
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
                "Razorpay payment verified successfully"
            ));
        }
    }
}

