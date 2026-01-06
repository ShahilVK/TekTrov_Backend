using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Order
{
    public class DirectOrderDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        // Shipping Details
        [Required] public string FullName { get; set; } = null!;
        [Required] public string PhoneNumber { get; set; } = null!;
        [Required] public string AddressLine { get; set; } = null!;
        [Required] public string City { get; set; } = null!;
        [Required] public string State { get; set; } = null!;
        [Required] public string PostalCode { get; set; } = null!;
        [Required] public string Country { get; set; } = null!;
    }
}
