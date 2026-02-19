namespace ProductManagementBackend.DTOs
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int Stock { get; set; }
        public DateTime AddedAt { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
    }
    public class AddToWishlistDto
    {
        public int ProductId { get; set; }
    }
}
