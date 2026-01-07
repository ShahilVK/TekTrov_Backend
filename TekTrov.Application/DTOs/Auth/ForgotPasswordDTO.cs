using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Auth
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
