namespace ContosoOnlineOrders.Abstractions.Models
{
    public record InventoryUpdateRequest(int productId, int countToAdd);
}