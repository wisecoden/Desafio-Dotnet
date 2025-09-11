using AvanadeAwesomeShop.Service.Orders.Domain.Events;

namespace AvanadeAwesomeShop.Service.Orders.Domain.Entities
{
    public class Customer : AggregateRoot
    {
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        // Construtor parameterless para EF
        private Customer()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Customer(string fullName, string email) : this()
        {
            FullName = fullName;
            Email = email;
        }


        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
