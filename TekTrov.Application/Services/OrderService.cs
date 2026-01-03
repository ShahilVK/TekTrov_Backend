using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Order;
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

        public async Task<List<OrderDTO>> GetOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null) return null;

            return new OrderDTO
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task PlaceOrderAsync(int userId)
        {
            var cartItems = await _cartRepository.GetByUserIdAsync(userId);
            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product!.Price
                });

                total += item.Product.Price * item.Quantity;
            }

            order.TotalAmount = total;

            await _orderRepository.AddAsync(order);
            await _cartRepository.RemoveRangeAsync(cartItems);
        }
    }
}
