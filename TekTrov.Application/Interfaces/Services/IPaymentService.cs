using TekTrov.Application.DTOs.Payments;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IPaymentService
    {
     
        //Task<object> HandleRazorpayPaymentAsync(int userId, RazorpayPaymentDTO dto);

        Task<RazorpayOrderResponseDTO> CreateRazorpayOrderAsync(decimal amount);
        Task<bool> VerifyPaymentAsync(RazorpayPaymentDTO dto);

    }
}
