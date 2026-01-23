using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Cart
{
    public class AddToCartDTO
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
