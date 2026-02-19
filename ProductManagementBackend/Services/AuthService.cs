using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductManagementBackend.Data;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;


namespace ProductManagementBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;


        public AuthService(AppDbContext db, IConfiguration config, IWebHostEnvironment env)
        {
            _db = db;
            _config = config;
            _env = env;
        }

        //public string Register(RegisterDto dto)
        //{
        //    if (_db.Users.Any(u => u.Username == dto.Username))
        //        throw new Exception("User already exists");

        //    var allowedRoles = new[] { "User", "Admin" };
        //    var role = allowedRoles.Contains(dto.Role) ? dto.Role : "User";

        //    var user = new User
        //    {
        //        Username = dto.Username,
        //        Email = dto.Email,
        //        PasswordHash = Hash(dto.Password),
        //        Role = role,
        //        IsApproved = role == "Admin",
        //        IsRestricted = false,
        //        IsEmailVerified = false
        //    };

        //    _db.Users.Add(user);
        //    _db.SaveChanges(); // ✅ save to database

        //    return "Registration successful";
        //}

        public async Task<string> Register(RegisterDto dto)
        {
            if (_db.Users.Any(u => u.Username == dto.Username))
                throw new Exception("User already exists");

            string? imagePath = null;

            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);
                var extension = Path.GetExtension(dto.ProfileImage.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";

                var fullPath = Path.Combine(uploadDir, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.ProfileImage.CopyToAsync(stream);

                imagePath = $"/uploads/{fileName}";
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = Hash(dto.Password),
                Role = dto.Role,
                ImagePath = imagePath,
                IsApproved = false,
                IsRestricted = false,
                IsEmailVerified = false
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return "Registration successful";
        }

        //public async Task<User?> UpdateUserAsync(int id, UpdateUserDto dto)
        //{
        //    var user = await _db.Users.FindAsync(id);
        //    if (user == null) return null;

        //    if (!string.IsNullOrEmpty(dto.Username))
        //        user.Username = dto.Username;

        //    if (dto.ProfileImage != null)
        //    {
        //        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);
        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await dto.ProfileImage.CopyToAsync(fileStream);
        //        }

        //        user.ImagePath = $"uploads/{uniqueFileName}";
        //    }

        //    _db.Users.Update(user);
        //    await _db.SaveChangesAsync();

        //    return user;
        //}
        public async Task<User?> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return null;

            // Update username
            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;

            // Update profile image
            if (dto.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(fileStream);
                }

                user.ImagePath = $"uploads/{uniqueFileName}";
            }

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return user;
        }


        public string Login(LoginDto dto, out string token, out string role, out bool isApproved, out bool isRestricted)
        {
            token = string.Empty;
            role = string.Empty;
            isApproved = false;
            isRestricted = false;

            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null || user.PasswordHash != Hash(dto.Password))
                throw new Exception("Invalid credentials");

            token = GenerateJwt(user);
            role = user.Role;
            isApproved = user.IsApproved;
            isRestricted = user.IsRestricted;

            return "Login successful";
        }

        public User? GetProfile(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Username == username);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public async Task<bool> ApproveUserAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null)
                return false;

            user.IsApproved = true;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestrictUserAsync(int userId, bool restrict)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null)
                return false;

            user.IsRestricted = restrict; // Set to the value passed (true or false)

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return true;
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
