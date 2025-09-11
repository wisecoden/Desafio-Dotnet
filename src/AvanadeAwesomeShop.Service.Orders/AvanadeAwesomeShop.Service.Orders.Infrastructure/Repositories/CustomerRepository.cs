using Microsoft.EntityFrameworkCore;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.Data;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OrdersDbContext _context;

        public CustomerRepository(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.OrderBy(c => c.FullName).ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            return await _context.Customers
                .Where(c => c.FullName.Contains(name))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await GetByIdAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Customers.CountAsync();
        }
    }
}
