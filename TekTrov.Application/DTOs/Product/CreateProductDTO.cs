using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekTrov.Application.DTOs.Products
{
    public class CreateProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Range(1, 10000)]
        public decimal Price { get; set; }

        [Required]
        //[RegularExpression(@"^(?!\s)[A-Za-z ]{2,50}$")]
        public string Category { get; set; } = null!;

        [Range(0, 10000)]
        public int Stock { get; set; }
    }
}
