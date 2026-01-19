
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
        private readonly IUserRepository _userRepository;


        public OrderService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
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
            await EnsureUserNotBlockedAsync(userId); // ✅ FIXED

            var cartItems = await _cartRepository.GetByUserIdAsync(userId);

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

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

            decimal total = 0;

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product!.Stock)
                    throw new Exception($"Insufficient stock for {item.Product.Name}");

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



        public async Task CancelOrderAsync(int userId, int orderId)
        {
            var order = await _orderRepository
                .GetOrderWithItemsAndProductsAsync(orderId, userId)
                ?? throw new Exception("Order not found");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered orders cannot be cancelled");

            if (order.Status == OrderStatus.Shipped)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Product!.Stock += item.Quantity;
                    await _productRepository.UpdateAsync(item.Product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);
        }



        public async Task<int> PlaceDirectOrderAsync(int userId, DirectOrderDTO dto)
        {
            await EnsureUserNotBlockedAsync(userId); // ✅ FIXED

            if (dto.Quantity <= 0)
                throw new Exception("Invalid quantity");

            var product = await _productRepository.GetByIdAsync(dto.ProductId)
                ?? throw new Exception("Product not found");

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

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Price = product.Price
            });

            order.TotalAmount = product.Price * dto.Quantity;

            await _orderRepository.AddAsync(order);

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


        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Cancelled order cannot be updated");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered order cannot be updated");

            if (order.Status == OrderStatus.Pending &&
                newStatus != OrderStatus.Shipped &&
                newStatus != OrderStatus.Cancelled)
                throw new Exception("Pending order can only be Shipped or Cancelled");

            if (order.Status == OrderStatus.Shipped &&
                newStatus != OrderStatus.Delivered)
                throw new Exception("Shipped order can only be Delivered");

            if (order.Status == OrderStatus.Shipped &&
                newStatus == OrderStatus.Delivered)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = item.Product
                        ?? throw new Exception("Product not found");

                    if (product.Stock < item.Quantity)
                        throw new Exception($"Insufficient stock for {product.Name}");

                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);
        }




        private async Task EnsureUserNotBlockedAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (user.IsBlocked)
                throw new Exception("Your account has been blocked. You cannot place orders.");
        }

    

        public async Task MarkOrderAsPaidAsync(int orderId, int userId)
        {
            var order = await _orderRepository
                .GetOrderWithItemsAndProductsAsync(orderId, userId)
                ?? throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be paid");

            foreach (var item in order.OrderItems)
            {
                if (item.Product!.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for {item.Product.Name}");
            }

            foreach (var item in order.OrderItems)
            {
                item.Product!.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(item.Product);
            }

            order.Status = OrderStatus.Shipped;
            order.ModifiedOn = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                        ?? throw new Exception("Order not found");

            await _orderRepository.DeleteAsync(order);
        }






    }
}
