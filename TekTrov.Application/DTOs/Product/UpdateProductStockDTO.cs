using System.ComponentModel.DataAnnotations;

namespace TekTrov.Application.DTOs.Products
{
    public class UpdateProductStockDTO
    {
        [Range(0, 100_000)]
        public int Stock { get; set; }
    }
}
