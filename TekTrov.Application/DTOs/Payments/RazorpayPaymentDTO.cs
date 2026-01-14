namespace TekTrov.Application.DTOs.Payments
{
    public class RazorpayPaymentDTO
    {
        public int OrderId { get; set; }

        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
    }
}
