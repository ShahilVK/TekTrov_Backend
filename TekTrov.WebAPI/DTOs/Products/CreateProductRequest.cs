using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TekTrov.WebApi.DTOs.Products
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Range(1, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = null!;

        [Range(0, 10000)]
        public int Stock { get; set; }

        public IFormFile? Image { get; set; }
    }
}
