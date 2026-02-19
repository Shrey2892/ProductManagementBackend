using Microsoft.EntityFrameworkCore;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;
using ProductManagementBackend.Data;
namespace ProductManagementBackend.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckoutSelectedAsync(int userId, List<int> selectedProductIds)
        {
            if (selectedProductIds == null || !selectedProductIds.Any())
                throw new ArgumentException("No items selected for checkout");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId && selectedProductIds.Contains(c.ProductId))
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("No items to checkout");

            foreach (var item in cartItems)
            {
                if (!item.Product.IsActive)
                    throw new InvalidOperationException($"{item.Product.Name} is no longer available");

                if (item.Product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for {item.Product.Name}");
            }

            foreach (var item in cartItems)
            {
                item.Product.StockQuantity -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId)
        {
            var items = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (!product.IsActive)
                throw new InvalidOperationException("Product is not available");

            if (product.StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            var existingItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;

                if (product.StockQuantity < existingItem.Quantity)
                    throw new InvalidOperationException("Insufficient stock");

                await _context.SaveChangesAsync();
                return MapToDto(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    IsSelected = false
                };

                _context.CartItems.Add(newItem);
                await _context.SaveChangesAsync();

                var savedItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstAsync(c => c.Id == newItem.Id);

                return MapToDto(savedItem);
            }
        }

        public async Task<CartItemDto> IncreaseQuantityAsync(int userId, int productId)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
                throw new KeyNotFoundException("Cart item not found");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (!product.IsActive)
                throw new InvalidOperationException("Product is not available");

            if (product.StockQuantity < cartItem.Quantity + 1)
                throw new InvalidOperationException("Insufficient stock");

            cartItem.Quantity++;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(cartItem);
        }

        public async Task<CartItemDto?> DecreaseQuantityAsync(int userId, int productId)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
                throw new KeyNotFoundException("Cart item not found");

            if (cartItem.Quantity <= 1)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return null;
            }

            cartItem.Quantity--;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(cartItem);
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckoutAsync(int userId, List<int>? selectedProductIds = null)
        {
            IQueryable<CartItem> query = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId);

            if (selectedProductIds != null && selectedProductIds.Any())
            {
                query = query.Where(c => selectedProductIds.Contains(c.ProductId));
            }

            var cartItems = await query.ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("No items to checkout");

            foreach (var item in cartItems)
            {
                if (!item.Product.IsActive)
                    throw new InvalidOperationException($"{item.Product.Name} is no longer available");

                if (item.Product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for {item.Product.Name}");
            }

            foreach (var item in cartItems)
            {
                item.Product.StockQuantity -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return true;
        }

        private static CartItemDto MapToDto(CartItem item)
        {
            return new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                IsSelected = item.IsSelected,
                Product = new ProductDto
                {
                    Id = item.Product.Id,
                    Name = item.Product.Name,
                    Description = item.Product.Description,
                    Price = item.Product.Price,
                    ImageUrl = item.Product.ImageUrl,
                    StockQuantity = item.Product.StockQuantity,
                    Category = item.Product.Category,
                    Sku = item.Product.Sku,
                    IsActive = item.Product.IsActive
                }
            };
        }
    }
}
