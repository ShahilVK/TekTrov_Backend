using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Common;
using TekTrov.Application.DTOs.Products;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Enums;

namespace TekTrov.WebApi.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("Admin-add-products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse("Validation failed", 400));

            await _productService.CreateProductAsync(dto);

            return StatusCode(
               201,
               ApiResponse<object>.SuccessResponse(
                   null,
                   "Product created successfully",
                   201
               ));
        }
    }
}
