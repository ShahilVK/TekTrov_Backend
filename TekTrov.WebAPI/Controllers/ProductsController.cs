using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Products;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;
using TekTrov.WebApi.DTOs.Products;

namespace TekTrov.WebApi.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;

        public ProductsController(
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
            return Ok(
               ApiResponse<object>.SuccessResponse(
                   products,
                   "Products fetched successfully"
               ));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(
                   ApiResponse<object>.FailureResponse(
                       "Product not found",
                       404
                   ));

            return Ok(
               ApiResponse<object>.SuccessResponse(
                   product,
                   "Product fetched successfully"
               ));
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService
                .GetProductsByCategoryAsync(category);

            if (products == null || products.Count == 0)
                return NotFound(
                    ApiResponse<object>.FailureResponse(
                        "No products found for this category",
                        404
                    ));

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    products,
                    "Products fetched successfully"
                ));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var products = await _productService.SearchProductsAsync(query);

            if (products.Count == 0)
                return NotFound(ApiResponse<object>.FailureResponse(
                    "No products found", 404));

            return Ok(ApiResponse<object>.SuccessResponse(
                products,
                "Products fetched successfully"
            ));
        }

[HttpGet("sort")]
public async Task<IActionResult> GetSortedProducts(
    [FromQuery] string sort)
{
    if (int.TryParse(sort, out _))
    {
        return BadRequest(ApiResponse<object>.FailureResponse(
            "Sort must be a valid name, not a number",
            400
        ));
    }

    if (!Enum.TryParse<ProductSortType>(
            sort,
            ignoreCase: true,
            out var sortType))
    {
        return BadRequest(ApiResponse<object>.FailureResponse(
            "Invalid sort option",
            400
        ));
    }

    var products = await _productService.GetSortedProductsAsync(sortType);

    return Ok(ApiResponse<object>.SuccessResponse(
        products,
        "Products sorted successfully"
    ));
}





        [Authorize(Roles = Roles.Admin)]
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
                ApiResponse<bool>.SuccessResponse(true, "Product created successfully", 201)
            );
        }


        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{productId:int}/stock")]
        public async Task<IActionResult> UpdateStock(int productId,[FromBody] UpdateProductStockDTO dto)
        {
            await _productService.UpdateStockAsync(productId, dto.Stock);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Product stock updated successfully"
            ));
        }


        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProductAsync(productId);

            return Ok(
                ApiResponse<bool>.SuccessResponse(
                    true,
                    "Product deleted successfully"
                )
            );
        }


    }
}
