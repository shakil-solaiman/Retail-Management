using RetailManagement.API.Models;

namespace RetailManagement.API.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);

        Task<Order?> GetByIdAsync(int id);

        Task<List<Order>> GetAllAsync(string? customerName, string? status, int pageNumber, int pageSize);

        Task<int> GetTotalCountAsync( string? customerName, string? status);

        Task<bool> CancelOrderAsync(int id);
    }
}