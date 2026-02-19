using Microsoft.EntityFrameworkCore;
using ProductManagementBackend.Data;
using ProductManagementBackend.Models;
using ProductManagementBackend.Data;
using ProductManagementBackend.Models;
using ProductManagementBackend.Services;
namespace ProductManagementBackend.Services
{
        public class ProductService : IProductService
        {
            private readonly AppDbContext _context;
            private readonly ILogger<ProductService> _logger;

            public ProductService(AppDbContext context, ILogger<ProductService> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<IEnumerable<Product>> GetAllProductsAsync()
            {
                try
                {
                    return await _context.Products
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching all products");
                    throw;
                }
            }

            public async Task<Product?> GetProductByIdAsync(int id)
            {
                try
                {
                    return await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching product with ID {ProductId}", id);
                    throw;
                }
            }

            public async Task<Product> CreateProductAsync(Product product)
            {
                try
                {
                    // Check if SKU already exists
                    if (await SkuExistsAsync(product.Sku))
                    {
                        throw new InvalidOperationException($"A product with SKU '{product.Sku}' already exists.");
                    }

                    product.CreatedAt = DateTime.UtcNow;
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
                    return product;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating product");
                    throw;
                }
            }

            public async Task<Product?> UpdateProductAsync(int id, Product product)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return null;
                    }

                    // Check if SKU already exists (excluding current product)
                    if (await SkuExistsAsync(product.Sku, id))
                    {
                        throw new InvalidOperationException($"A product with SKU '{product.Sku}' already exists.");
                    }

                    // Update properties
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.Sku = product.Sku;
                    existingProduct.Category = product.Category;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.IsActive = product.IsActive;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product updated successfully with ID {ProductId}", id);
                    return existingProduct;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error occurred while updating product with ID {ProductId}", id);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}", id);
                    throw;
                }
            }

            public async Task<bool> DeleteProductAsync(int id)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return false;
                    }

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product deleted successfully with ID {ProductId}", id);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}", id);
                    throw;
                }
            }

            public async Task<bool> ProductExistsAsync(int id)
            {
                return await _context.Products.AnyAsync(p => p.Id == id);
            }

            public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
            {
                if (excludeId.HasValue)
                {
                    return await _context.Products
                        .AnyAsync(p => p.Sku == sku && p.Id != excludeId.Value);
                }

                return await _context.Products.AnyAsync(p => p.Sku == sku);
            }
        }
    }

