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

        [HttpGet("paged")]
        public async Task<IActionResult> GetProductsPaged(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var result = await _productService
                .GetProductsPagedAsync(pageNumber, pageSize);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Products fetched successfully"
            ));
        }
    }
}
