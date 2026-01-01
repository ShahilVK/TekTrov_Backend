using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersAsync(int userId);

        Task<Order?> GetOrderByIdAsync(int orderId, int userId);

        Task PlaceOrderAsync(int userId);
    }
}
