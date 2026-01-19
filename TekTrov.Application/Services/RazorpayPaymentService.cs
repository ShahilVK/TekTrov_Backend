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
        }


        private static string GenerateSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }



        public Task<RazorpayOrderResponseDTO> CreateRazorpayOrderAsync(
         decimal amount)
        {
            var client = new RazorpayClient(
                _settings.KeyId,
                _settings.KeySecret
            );

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, // paise
                { "currency", "INR" },
                { "receipt", $"txn_{Guid.NewGuid()}" }
            };

            var razorpayOrder = client.Order.Create(options);

            return Task.FromResult(new RazorpayOrderResponseDTO
            {
                RazorpayOrderId = razorpayOrder["id"].ToString(),
                Amount = amount
            });
        }

        public Task<bool> VerifyPaymentAsync(RazorpayPaymentDTO dto)
        {
            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            var generatedSignature =
                GenerateSignature(payload, _settings.KeySecret);

            if (generatedSignature != dto.RazorpaySignature)
                throw new Exception("Invalid Razorpay signature");

            return Task.FromResult(true);
        }




    }
}
