using System.ComponentModel.DataAnnotations;
using TekTrov.Domain.Enums;

namespace TekTrov.Application.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
