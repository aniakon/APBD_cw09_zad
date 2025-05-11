using Microsoft.Data.SqlClient;

namespace APBD_cw09.Services;

public class ProductsService : IProductsService
{
    private readonly IConfiguration _configuration;
    
    public ProductsService(IConfiguration config)
    {
        _configuration = config;
    }
    
    // checks if product with given id exists in Product table
    public async Task<bool> DoesProductExist(int productId)
    {
        string query = @"SELECT 1 FROM Product WHERE IdProduct = @idProduct";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", productId);
            
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }
    }

    public async Task<double> GetProductPrice(int productId)
    {
        string query = @"SELECT Price FROM Product WHERE IdProduct = @idProduct";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", productId);
            
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToDouble(res);
        }
    }
}