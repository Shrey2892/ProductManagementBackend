using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;

namespace ProductManagementBackend.Services
{
    public interface IAuthService
    {
        //string Register(RegisterDto dto);
        Task<string> Register(RegisterDto dto);

        string Login(LoginDto dto, out string token, out string role, out bool isApproved, out bool isRestricted);
        IEnumerable<User> GetAllUsers();
        User? GetProfile(string username);
        Task<User?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> ApproveUserAsync(int userId);
        Task<bool> RestrictUserAsync(int userId, bool restrict);
    }
}
