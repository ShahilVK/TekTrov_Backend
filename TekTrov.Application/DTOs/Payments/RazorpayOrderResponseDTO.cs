namespace TekTrov.Application.DTOs.Payments
{
    public class RazorpayOrderResponseDTO
    {
        public string RazorpayOrderId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
