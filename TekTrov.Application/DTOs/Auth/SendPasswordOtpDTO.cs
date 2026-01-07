using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Auth
{
    public class SendPasswordOtpDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
