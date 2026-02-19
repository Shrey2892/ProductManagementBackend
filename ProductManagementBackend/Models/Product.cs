using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementBackend.Models
{     public class Product
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(200)]
            public string Name { get; set; } = string.Empty;

            [StringLength(1000)]
            public string Description { get; set; } = string.Empty;

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal Price { get; set; }

            [Required]
            public int StockQuantity { get; set; }

            [Required]
            [StringLength(100)]
            public string Sku { get; set; } = string.Empty;

            [StringLength(100)]
            public string Category { get; set; } = string.Empty;

            [StringLength(500)]
            public string ImageUrl { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }

