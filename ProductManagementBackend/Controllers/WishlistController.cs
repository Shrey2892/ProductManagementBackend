using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Services;
using System.Security.Claims;

namespace ProductManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: api/wishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/wishlist
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var result = await _wishlistService.AddToWishlistAsync(userId, dto.ProductId);

                if (!result)
                    return BadRequest("Product already in wishlist or product not found");

                return Ok(new { message = "Product added to wishlist successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/wishlist/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);

                if (!result)
                    return NotFound("Product not found in wishlist");

                return Ok(new { message = "Product removed from wishlist successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/wishlist/check/{productId}
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
                return Ok(new { isInWishlist });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/wishlist/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var result = await _wishlistService.ClearWishlistAsync(userId);

                if (!result)
                    return BadRequest("Wishlist is already empty");

                return Ok(new { message = "Wishlist cleared successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}