namespace TekTrov.Application.DTOs.Products
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
