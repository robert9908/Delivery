using Delivery.Models.Domain;

namespace Delivery.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync(DateTime? fromDate = null, string? district = null);
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(int id, Order order);
        Task<Order?> DeleteAsync(int id);
    }
}
