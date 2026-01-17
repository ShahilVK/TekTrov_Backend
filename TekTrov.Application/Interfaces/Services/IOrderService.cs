using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Order;
using TekTrov.Domain.Entities;
using TekTrov.Domain.Enums;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetOrdersAsync(int userId);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId, int userId);
        //Task PlaceOrderAsync(int userId, CheckoutDTO dto);
        Task<int> PlaceOrderAsync(int userId, CheckoutDTO dto);

        Task<int> PlaceDirectOrderAsync(int userId, DirectOrderDTO dto);

        Task CancelOrderAsync(int userId, int orderId);

        Task<List<AdminOrderDTO>> GetAllOrdersForAdminAsync();
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task MarkOrderAsPaidAsync(int orderId, int userId);

         Task DeleteOrderAsync(int orderId);

    }
}
