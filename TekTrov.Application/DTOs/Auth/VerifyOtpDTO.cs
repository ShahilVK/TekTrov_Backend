using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Auth
{
    public class VerifyOtpDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; } = null!;
    }
}
