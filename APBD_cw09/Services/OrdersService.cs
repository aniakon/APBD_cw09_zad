using System.Data;
using Microsoft.Data.SqlClient;

namespace APBD_cw09.Services;

public class OrdersService : IOrdersService
{
    private readonly IConfiguration _configuration;
    
    public OrdersService(IConfiguration config)
    {
        _configuration = config;
    }
    
    // returns id of the order of given product with given amount (if != null, then order exists)
    public async Task<int?> GetOrderId(int productId, int amount)
    {
        string query = @"SELECT IdOrder FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", productId);
            cmd.Parameters.AddWithValue("@amount", amount);
            
            await conn.OpenAsync();
            
            var res = await cmd.ExecuteScalarAsync(); // return first that is matching
            if (res == null) return null;
            return Convert.ToInt32(res);
        }
    }

    // checks if given date is after the date of creation of the order
    public async Task<bool> IsOrderDateEarlier(int id, int amount, string givenDate)
    {
        string query = @"SELECT CreatedAt FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", id);
            cmd.Parameters.AddWithValue("@amount", amount);
            
            await conn.OpenAsync();
            
            var date = await cmd.ExecuteScalarAsync();
            return Convert.ToDateTime(date) < Convert.ToDateTime(givenDate);
        }
    }

    public async Task<bool> UpdateDate(int orderId)
    {
        string query = @"UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@IdOrder", orderId);
            cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
            
            await conn.OpenAsync();
            
            var res = await cmd.ExecuteNonQueryAsync();
            return Convert.ToBoolean(res);
        }
    }
}