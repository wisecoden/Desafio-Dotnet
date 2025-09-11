using AvanadeAwesomeShop.Service.Orders.Domain.Entities;

namespace AvanadeAwesomeShop.Service.Orders.Domain.Repositories
{
    public interface ICustomerRepository
    {
        // Queries
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> SearchByNameAsync(string name);
        
        // Commands
        Task<Customer> CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        
        // Checks
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<int> CountAsync();
    }
}
