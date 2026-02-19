using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Services;
using System.Security.Claims;

namespace ProductManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[HttpPost("register")]
        //public IActionResult Register([FromForm] RegisterDto dto)
        //{
        //    try
        //    {
        //        var message = _authService.Register(dto);
        //        return Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            try
            {
                var message = await _authService.Register(dto); // ✅ await
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var message = _authService.Login(dto, out string token, out string role, out bool isApproved, out bool isRestricted);
                return Ok(new
                {
                    token,
                    role,
                    isApproved,
                    isRestricted
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
                return Unauthorized("Invalid or expired token");

            var user = _authService.GetProfile(username);

            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.IsApproved,
                user.IsRestricted,
                user.IsEmailVerified,
                user.ImagePath
            });
        }

        //[HttpPut("update/{id}")]
        //[Authorize] // You can restrict further if needed
        //public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserDto dto)
        //{
        //    try
        //    {
        //        var updatedUser = await _authService.UpdateUserAsync(id, dto);
        //        if (updatedUser == null)
        //            return NotFound("User not found");

        //        return Ok(updatedUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPut("update")]
        [Authorize] // You can allow Admins to update any user or Users to update their own
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto dto)
        {
            try
            {
                // For regular users, get userId from token
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim);

                var updatedUser = await _authService.UpdateUserAsync(userId, dto);
                if (updatedUser == null)
                    return NotFound("User not found");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("approve/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveUser(int userId)
        {
            try
            {
                var result = await _authService.ApproveUserAsync(userId);

                if (!result)
                    return NotFound("User not found");

                return Ok(new { message = "User approved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("restrict/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestrictUser(int userId, [FromQuery] bool restrict)
        {
            try
            {
                var result = await _authService.RestrictUserAsync(userId, restrict);

                if (!result)
                    return NotFound("User not found");

                var message = restrict
                    ? "User restricted successfully"
                    : "User unrestricted successfully";

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers() => Ok(_authService.GetAllUsers());

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

    }
}
