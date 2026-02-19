using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;
using ProductManagementBackend.Services;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;
using ProductManagementBackend.Services;
namespace ProductManagementBackend.Controllers
{
         [Route("api/[controller]")]
        [ApiController]
        [Authorize] // Add this if you're using JWT authentication
        public class ProductsController : ControllerBase
        {
            private readonly IProductService _productService;
            private readonly ILogger<ProductsController> _logger;

            public ProductsController(IProductService productService, ILogger<ProductsController> logger)
            {
                _productService = productService;
                _logger = logger;
            }

            // GET: api/Products
            [HttpGet]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
            {
                try
                {
                    var products = await _productService.GetAllProductsAsync();
                    var productDtos = products.Select(p => MapToResponseDto(p));
                    return Ok(productDtos);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching all products");
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            // GET: api/Products/5
            [HttpGet("{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
            {
                try
                {
                    var product = await _productService.GetProductByIdAsync(id);

                    if (product == null)
                    {
                        return NotFound($"Product with ID {id} not found.");
                    }

                    return Ok(MapToResponseDto(product));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching product with ID {ProductId}", id);
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            // POST: api/Products
            [HttpPost]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto createDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    var product = MapToProduct(createDto);
                    var createdProduct = await _productService.CreateProductAsync(product);

                    var responseDto = MapToResponseDto(createdProduct);
                    return CreatedAtAction(
                        nameof(GetProduct),
                        new { id = createdProduct.Id },
                        responseDto);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating product");
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            // PUT: api/Products/5
            [HttpPut("{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    var product = MapToProduct(updateDto);
                    var updatedProduct = await _productService.UpdateProductAsync(id, product);

                    if (updatedProduct == null)
                    {
                        return NotFound($"Product with ID {id} not found.");
                    }

                    return Ok(MapToResponseDto(updatedProduct));
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}", id);
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            // DELETE: api/Products/5
            [HttpDelete("{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> DeleteProduct(int id)
            {
                try
                {
                    var result = await _productService.DeleteProductAsync(id);

                    if (!result)
                    {
                        return NotFound($"Product with ID {id} not found.");
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}", id);
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            #region Helper Methods

            private static ProductResponseDto MapToResponseDto(Product product)
            {
                return new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Sku = product.Sku,
                    Category = product.Category,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt
                };
            }

            private static Product MapToProduct(CreateProductDto dto)
            {
                return new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity,
                    Sku = dto.Sku,
                    Category = dto.Category,
                    ImageUrl = dto.ImageUrl,
                    IsActive = dto.IsActive
                };
            }

            private static Product MapToProduct(UpdateProductDto dto)
            {
                return new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity,
                    Sku = dto.Sku,
                    Category = dto.Category,
                    ImageUrl = dto.ImageUrl,
                    IsActive = dto.IsActive
                };
            }

            #endregion
        }
    }
