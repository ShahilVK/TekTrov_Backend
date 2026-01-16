using Microsoft.Extensions.Options;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Payments;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;

namespace TekTrov.Application.Services
{
    public class RazorpayPaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly RazorpaySettings _settings;


        public RazorpayPaymentService(
            IOptions<RazorpaySettings> options,
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _settings = options.Value;

            Console.WriteLine("=== RAZORPAY DEBUG ===");
            Console.WriteLine("KeyId: " + _settings.KeyId);
            Console.WriteLine("KeySecret: " + _settings.KeySecret);
        }


        private static string GenerateSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public async Task<object> HandleRazorpayPaymentAsync(
    int userId,
    RazorpayPaymentDTO dto)
{
    if (string.IsNullOrWhiteSpace(dto.RazorpayOrderId))
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId, userId)
            ?? throw new Exception("Order not found");

        if (order.Status != OrderStatus.Pending)
            throw new Exception("Order already processed");

        var client = new RazorpayClient(
            _settings.KeyId,
            _settings.KeySecret);

        var options = new Dictionary<string, object>
        {
            { "amount", (int)(order.TotalAmount * 100) },
            { "currency", "INR" },
            { "receipt", $"order_{order.Id}" }
        };

        var razorpayOrder = client.Order.Create(options);

        return new RazorpayOrderResponseDTO
        {
            RazorpayOrderId = razorpayOrder["id"].ToString(),
            Amount = order.TotalAmount
        };
    }

    var existingOrder = await _orderRepository.GetByIdAsync(dto.OrderId, userId)
        ?? throw new Exception("Order not found");

    string payload =
        $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

    string generatedSignature =
        GenerateSignature(payload, _settings.KeySecret);

    if (generatedSignature != dto.RazorpaySignature)
        throw new Exception("Payment verification failed");

    existingOrder.Status = OrderStatus.Pending;
    existingOrder.ModifiedOn = DateTime.UtcNow;

    await _orderRepository.UpdateAsync(existingOrder);

    return true;
}

    }
}
