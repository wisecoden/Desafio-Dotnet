﻿namespace AvanadeAwesomeShop.Service.Orders.Application.Dtos

{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
