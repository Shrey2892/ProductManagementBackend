namespace ProductManagementBackend.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = null!;
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class CheckoutDto
    {
        public List<int>? ProductIds { get; set; }
    }

    public class CheckoutSelectedDto
    {
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
