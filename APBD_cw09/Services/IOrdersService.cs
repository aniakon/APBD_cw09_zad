namespace APBD_cw09.Services;

public interface IOrdersService
{
    public Task<int?> GetOrderId(int id, int amount);
    
    // checks if date given by user is after the one describing creation of order
    public Task<bool> IsOrderDateEarlier(int id, int amount, string givenDate);

    // if result != null, then updating was successful
    public Task<bool> UpdateDate(int orderId);
}