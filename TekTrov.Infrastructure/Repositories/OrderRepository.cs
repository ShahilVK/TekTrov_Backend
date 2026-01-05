//using ECommerce.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TekTrov.Application.Interfaces.Repositories;
//using TekTrov.Domain.Entities;

//namespace TekTrov.Infrastructure.Repositories
//{
//    public class OrderRepository : IOrderRepository
//    {
//        private readonly AppDbContext _context;

//        public OrderRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<Order>> GetByUserIdAsync(int userId)
//        {
//            return await _context.Orders
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .Where(o => o.UserId == userId)
//                .OrderByDescending(o => o.OrderDate)
//                .ToListAsync();
//        }

//        public async Task<Order?> GetByIdAsync(int orderId, int userId)
//        {
//            return await _context.Orders
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .FirstOrDefaultAsync(o =>
//                    o.Id == orderId &&
//                    o.UserId == userId);
//        }

//        public async Task AddAsync(Order order)
//        {
//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(Order order)
//        {
//            _context.Orders.Update(order);
//            await _context.SaveChangesAsync();
//        }

//    }
//}
