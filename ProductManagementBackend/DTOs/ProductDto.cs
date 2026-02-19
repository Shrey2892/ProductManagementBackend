using System.ComponentModel.DataAnnotations;
namespace ProductManagementBackend.DTOs
{
        public class CreateProductDto
        {
            [Required(ErrorMessage = "Product name is required")]
            [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
            public string Name { get; set; } = string.Empty;

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Stock quantity is required")]
            [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
            public int StockQuantity { get; set; }

            [Required(ErrorMessage = "SKU is required")]
            [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
            public string Sku { get; set; } = string.Empty;

            [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
            public string Category { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
            [Url(ErrorMessage = "Invalid URL format")]
            public string ImageUrl { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }

        public class UpdateProductDto
        {
            [Required(ErrorMessage = "Product name is required")]
            [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
            public string Name { get; set; } = string.Empty;

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Stock quantity is required")]
            [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
            public int StockQuantity { get; set; }

            [Required(ErrorMessage = "SKU is required")]
            [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
            public string Sku { get; set; } = string.Empty;

            [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
            public string Category { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
            [Url(ErrorMessage = "Invalid URL format")]
            public string ImageUrl { get; set; } = string.Empty;

            public bool IsActive { get; set; }
        }

        public class ProductResponseDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public string Sku { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

