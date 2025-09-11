using AvanadeAwesomeShop.Service.Orders.Domain.Enums;
using AvanadeAwesomeShop.Service.Orders.Domain.Events;

namespace AvanadeAwesomeShop.Service.Orders.Domain.Entities
{
    public class Order : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // Construtor parameterless para EF
        private Order()
        {
            Id = Guid.NewGuid();
            Status = OrderStatus.Started;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Order(Guid customerId) : this()
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID vazio", nameof(customerId));

            CustomerId = customerId;
        }

        
        public void AddItem(Guid productId, int quantity, decimal price)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID é obrigatório", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

            if (price <= 0)
                throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(price));

            // Verificar se já existe item com mesmo produto
            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var item = new OrderItem(productId, quantity, price);
                _items.Add(item);
            }

            CalculateTotal();

        }

        public void CreateOrder()
        {
            // Disparar evento quando o pedido for criado
            AddDomainEvent(new OrderCreatedEvent(Id, CustomerId, TotalPrice, _items.Count));
        }

        public void ConfirmOrder()
        {
            Status = OrderStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            // Disparar evento de confirmação com dados dos itens para reduzir estoque
            var itemsData = _items.Select(i => new OrderItemData(i.ProductId, i.Quantity, i.Price)).ToList();
            AddDomainEvent(new OrderConfirmedEvent(Id, CustomerId, TotalPrice, itemsData));
        }

        public void CancelOrder(string reason)
        {

            Status = OrderStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new OrderCancelledEvent(Id, CustomerId, reason, TotalPrice));
        }

        private void CalculateTotal()
        {
            TotalPrice = Items.Sum(i => i.Quantity * i.Price);
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
