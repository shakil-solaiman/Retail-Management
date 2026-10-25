using RetailManagement.API.Models;

namespace RetailManagement.API.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);

        Task<List<Product>> GetAllAsync( string? search, int pageNumber, int pageSize);

        Task<int> GetTotalCountAsync(string? search);

        Task<Product?> GetByIdAsync(int id);

        Task<bool> CodeExistsAsync(string code,int? excludeProductId = null);

        Task<bool> UpdateAsync(Product product);

        Task<bool> SoftDeleteAsync(int id);
    }
}