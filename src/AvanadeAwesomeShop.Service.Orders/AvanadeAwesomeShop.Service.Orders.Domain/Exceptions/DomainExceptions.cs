namespace AvanadeAwesomeShop.Service.Orders.Domain.Exceptions
{
    public class OrderDomainException : Exception
    {
        public OrderDomainException(string message) : base(message) { }
        
        public OrderDomainException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    public class OrderNotFoundException : OrderDomainException
    {
        public OrderNotFoundException(Guid orderId) 
            : base($"Pedido com ID {orderId} não foi encontrado") { }
    }

    public class InvalidOrderStatusException : OrderDomainException
    {
        public InvalidOrderStatusException(string operation, string currentStatus) 
            : base($"Não é possível {operation} um pedido com status {currentStatus}") { }
    }

    public class EmptyOrderException : OrderDomainException
    {
        public EmptyOrderException() 
            : base("Pedido deve ter pelo menos um item") { }
    }


    public class InvalidEmailException : OrderDomainException
    {
        public InvalidEmailException(string email) 
            : base($"Email {email} é inválido") { }
    }
}
