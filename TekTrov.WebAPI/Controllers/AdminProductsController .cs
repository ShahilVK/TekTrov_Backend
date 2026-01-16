using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Products;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;
using TekTrov.WebApi.DTOs.Products;



namespace TekTrov.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = Roles.Admin)]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;

        public AdminProductsController(
            IProductService productService,
            IImageService imageService)
        {
            _productService = productService;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                products,
                "Products fetched successfully"
            ));
        }

        [HttpPost("Admin-add-products")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            string? imageUrl = null;

            if (request.Image != null)
            {
                imageUrl = await _imageService.UploadAsync(
                    request.Image.OpenReadStream(),
                    request.Image.FileName,
                    "products"
                );
            }

            var dto = new CreateProductDTO
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = request.Category,
                Stock = request.Stock
            };

            await _productService.CreateProductAsync(dto, imageUrl);

            return StatusCode(201,
                ApiResponse<bool>.SuccessResponse(
                    true, "Product created successfully", 201));
        }

        [HttpPut("{productId:int}/stock")]
        public async Task<IActionResult> UpdateStock(
            int productId,
            [FromBody] UpdateProductStockDTO dto)
        {
            await _productService.UpdateStockAsync(productId, dto.Stock);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Product stock updated successfully"));
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProductAsync(productId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Product deleted successfully"));
        }
    }

}
