using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;

namespace TekTrov.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;


        public OrderService(
        ICartRepository cartRepository,
        IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetOrdersAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
        {
            return await _orderRepository.GetByIdAsync(orderId, userId);
        }

        public async Task PlaceOrderAsync(int userId)
        {
            var cartItems = await _cartRepository.GetByUserIdAsync(userId);

            if (cartItems.Count == 0)
                throw new Exception("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product!.Price
                };

                total += orderItem.Price * orderItem.Quantity;
                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = total;

            await _orderRepository.AddAsync(order);

            // ✅ Clear cart after successful checkout
            await _cartRepository.RemoveRangeAsync(cartItems);
        }
    }
}
