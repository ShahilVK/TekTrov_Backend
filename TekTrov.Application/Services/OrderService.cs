
using TekTrov.Application.DTOs.Order;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;
using TekTrov.Domain.Enums;

namespace TekTrov.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;



        public OrderService(
     ICartRepository cartRepository,
     IOrderRepository orderRepository,
     IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }


        public async Task<List<OrderDTO>> GetOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                Items = o.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl
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
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl
                }).ToList()
            };
        }

        public async Task<int> PlaceOrderAsync(int userId, CheckoutDTO dto)
        {
            var cartItems = await _cartRepository.GetByUserIdAsync(userId);

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,

                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country
            };

            decimal total = 0;

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product!.Stock)
                    throw new Exception($"Insufficient stock for {item.Product.Name}");

                item.Product.Stock -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });

                total += item.Product.Price * item.Quantity;
            }

            order.TotalAmount = total;

            await _orderRepository.AddAsync(order);
            await _cartRepository.RemoveRangeAsync(cartItems);
            return order.Id;
        }


     //   public async Task PayOrderAsync(
     //int userId,
     //int orderId,
     //OrderPaymentDTO dto)
     //   {
     //       var order = await _orderRepository.GetByIdAsync(orderId, userId);

     //       if (order == null)
     //           throw new Exception("Order not found");

     //       if (order.Status == OrderStatus.Paid)
     //           throw new Exception("Order already paid");

     //       if (order.Status == OrderStatus.Cancelled)
     //           throw new Exception("Cancelled order cannot be paid");

     //       // ✅ Payment validation (basic)
     //       if (string.IsNullOrWhiteSpace(dto.TransactionId))
     //           throw new Exception("Invalid transaction");

     //       order.Status = OrderStatus.Paid;
     //       order.ModifiedOn = DateTime.UtcNow;

     //       await _orderRepository.UpdateAsync(order);
     //   }


        public async Task CancelOrderAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be cancelled");

            foreach (var item in order.OrderItems)
            {
                var product = item.Product!;
                product.Stock += item.Quantity;

                await _productRepository.UpdateAsync(product);
            }

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<int>  PlaceDirectOrderAsync(int userId, DirectOrderDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            if (product.Stock < dto.Quantity)
                throw new Exception("Insufficient stock");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,

                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country
            };

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Price = product.Price
            };

            order.OrderItems.Add(orderItem);
            order.TotalAmount = product.Price * dto.Quantity;

            product.Stock -= dto.Quantity;

            await _orderRepository.AddAsync(order);
            await _productRepository.UpdateAsync(product);
            return order.Id;
        }

        public async Task<List<AdminOrderDTO>> GetAllOrdersForAdminAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(o => new AdminOrderDTO
            {
                OrderId = o.Id,
                UserId = o.UserId,
                UserEmail = o.User!.Email,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                Items = o.OrderItems.Select(i => new AdminOrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl
                }).ToList()
            }).ToList();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Cancelled order cannot be updated");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered order cannot be updated");

            order.Status = status;

            await _orderRepository.UpdateAsync(order);
        }



    }
}
