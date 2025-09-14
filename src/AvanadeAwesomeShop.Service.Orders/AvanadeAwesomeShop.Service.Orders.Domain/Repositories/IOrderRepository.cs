using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Domain.Repositories
{
    public interface IOrderRepository
    {
        // Queries
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<Order?> GetOrderWithItemsByIdAsync(Guid id);
        
        // Commands
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
        
        // Checks
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountByCustomerAsync(Guid customerId);
        Task<decimal> GetTotalPriceByCustomerAsync(Guid customerId);
    }
}
