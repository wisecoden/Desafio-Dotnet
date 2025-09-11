namespace AvanadeAwesomeShop.Service.Orders.Application.Dtos
{   
    
    public class CreateOrderDto
    {
        //Cliente cadastrado, o melhor é passar o Id do cliente
        //  public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }


    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
 
   }

}
