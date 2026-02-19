using ProductManagementBackend.DTOs;

namespace ProductManagementBackend.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId);
        Task<CartItemDto> AddToCartAsync(int userId, int productId, int quantity = 1);
        Task<CartItemDto> IncreaseQuantityAsync(int userId, int productId);
        Task<CartItemDto?> DecreaseQuantityAsync(int userId, int productId);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task<bool> ClearCartAsync(int userId);
        Task<bool> CheckoutAsync(int userId, List<int>? selectedProductIds = null);
        Task<bool> CheckoutSelectedAsync(int userId, List<int> selectedProductIds);
    }
}
