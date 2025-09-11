using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Application.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDescription { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public int ItemsCount => Items.Count;
    }
}
