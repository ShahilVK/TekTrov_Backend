using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetByUserIdAsync(int userId);

        Task<Order?> GetByIdAsync(int orderId, int userId);

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);
    }
}
