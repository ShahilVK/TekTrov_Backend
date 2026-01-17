using TekTrov.Application.DTOs.Payments;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IPaymentService
    {
     
        Task<object> HandleRazorpayPaymentAsync(int userId, RazorpayPaymentDTO dto);

        Task<RazorpayOrderResponseDTO> CreateRazorpayOrderAsync(int userId, int orderId);

    }
}
