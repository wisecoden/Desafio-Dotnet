namespace AvanadeAwesomeShop.Service.Orders.Application.Dtos
{   
    
    public class CreateOrderDto
    {
         public Guid CustomerId { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }


    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
 
   }

}
