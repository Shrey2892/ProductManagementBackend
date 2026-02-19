using ProductManagementBackend.Models;
using ProductManagementBackend.Models;
namespace ProductManagementBackend.Services
{
   
        public interface IProductService
        {
            Task<IEnumerable<Product>> GetAllProductsAsync();
            Task<Product?> GetProductByIdAsync(int id);
            Task<Product> CreateProductAsync(Product product);
            Task<Product?> UpdateProductAsync(int id, Product product);
            Task<bool> DeleteProductAsync(int id);
            Task<bool> ProductExistsAsync(int id);
            Task<bool> SkuExistsAsync(string sku, int? excludeId = null);
        }
    }

