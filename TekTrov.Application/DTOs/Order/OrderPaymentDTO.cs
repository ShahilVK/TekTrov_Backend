using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Order
{
    public class OrderPaymentDTO
    {
        [Required]
        public string PaymentMethod { get; set; } = null!;
        // Example: "UPI", "CARD", "COD"

        [Required]
        public string TransactionId { get; set; } = null!;

        public string? Note { get; set; }
    }
}
