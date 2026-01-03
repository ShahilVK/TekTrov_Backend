using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Order;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetOrdersAsync(int userId);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId, int userId);
        Task PlaceOrderAsync(int userId);
    }
}
