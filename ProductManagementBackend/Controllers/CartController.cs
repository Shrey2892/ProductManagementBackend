using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Services;
using System.Security.Claims;

namespace ProductManagementBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // ✅ Extract logged-in user ID from JWT
        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var items = await _cartService.GetCartItemsAsync(userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve cart", error = ex.Message });
            }
        }

        [HttpPost("add/{productId}")]
        public async Task<ActionResult<CartItemDto>> AddToCart(int productId, [FromQuery] int quantity = 1)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _cartService.AddToCartAsync(userId, productId, quantity);
                return Ok(cartItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add to cart", error = ex.Message });
            }
        }

        [HttpPut("increase/{productId}")]
        public async Task<ActionResult<CartItemDto>> IncreaseQuantity(int productId)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _cartService.IncreaseQuantityAsync(userId, productId);
                return Ok(cartItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update quantity", error = ex.Message });
            }
        }

        [HttpPut("decrease/{productId}")]
        public async Task<ActionResult<CartItemDto>> DecreaseQuantity(int productId)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _cartService.DecreaseQuantityAsync(userId, productId);

                if (cartItem == null)
                    return NoContent();

                return Ok(cartItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update quantity", error = ex.Message });
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _cartService.RemoveFromCartAsync(userId, productId);

                if (!result)
                    return NotFound(new { message = "Cart item not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to remove from cart", error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to clear cart", error = ex.Message });
            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto? dto = null)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.CheckoutAsync(userId, dto?.ProductIds);
                return Ok(new { message = "Checkout successful" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Checkout failed", error = ex.Message });
            }
        }

        [HttpPost("checkout-selected")]
        public async Task<IActionResult> CheckoutSelected([FromBody] CheckoutSelectedDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.CheckoutSelectedAsync(userId, dto.ProductIds);
                return Ok(new { message = "Checkout successful" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Checkout failed", error = ex.Message });
            }
        }
    }
}

