namespace TekTrov.Application.DTOs.Order
{
    public class AdminOrderDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public List<AdminOrderItemDTO> Items { get; set; } = new();
    }
}
