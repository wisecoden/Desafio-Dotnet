namespace AvanadeAwesomeShop.Service.Orders.Application.Services
{
    public interface IStockNotificationService
    {
        Task NotifyStockReductionAsync(Guid productId, int quantity, Guid orderId);
    }
}
