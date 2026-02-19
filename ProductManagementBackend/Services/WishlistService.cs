using Microsoft.EntityFrameworkCore;
using ProductManagementBackend.Data;
using ProductManagementBackend.DTOs;
using ProductManagementBackend.Models;

namespace ProductManagementBackend.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _db;

        public WishlistService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WishlistItemDto>> GetUserWishlistAsync(int userId)
        {
            return await _db.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Select(w => new WishlistItemDto
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Product!.Name,
                    ProductDescription = w.Product.Description,
                    Price = w.Product.Price,
                    ImagePath = w.Product.ImageUrl,
                    Category = w.Product.Category,
                    Stock = w.Product.StockQuantity,
                    IsActive = w.Product.IsActive,
                    AddedAt = w.AddedAt
                })
                .ToListAsync();
        }

        public async Task<bool> AddToWishlistAsync(int userId, int productId)
        {
            // Check if product exists
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
                return false;

            // Check if already in wishlist
            var exists = await _db.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            if (exists)
                return false; // Already in wishlist

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            _db.Wishlists.Add(wishlistItem);
            await _db.SaveChangesAsync();

            return true;
        }

        //public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        //{
        //    var wishlistItem = await _db.Wishlists
        //        .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        //    if (wishlistItem == null)
        //        return false;

        //    _db.Wishlists.Remove(wishlistItem);
        //    await _db.SaveChangesAsync();

        //    return true;
        //}
        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            // ✅ Add logging
            Console.WriteLine($"Service - Looking for UserId: {userId}, ProductId: {productId}");

            var wishlistItem = await _db.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            // ✅ Add logging
            if (wishlistItem == null)
            {
                Console.WriteLine("Wishlist item not found in database");

                // Let's check what's actually in the database
                var allUserWishlistItems = await _db.Wishlists
                    .Where(w => w.UserId == userId)
                    .ToListAsync();
                Console.WriteLine($"User has {allUserWishlistItems.Count} wishlist items");
                foreach (var item in allUserWishlistItems)
                {
                    Console.WriteLine($"  - WishlistId: {item.Id}, ProductId: {item.ProductId}");
                }

                return false;
            }

            Console.WriteLine($"Found wishlist item with Id: {wishlistItem.Id}");

            _db.Wishlists.Remove(wishlistItem);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            return await _db.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<bool> ClearWishlistAsync(int userId)
        {
            var wishlistItems = await _db.Wishlists
                .Where(w => w.UserId == userId)
                .ToListAsync();

            if (!wishlistItems.Any())
                return false;

            _db.Wishlists.RemoveRange(wishlistItems);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}