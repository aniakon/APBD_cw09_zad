using Microsoft.Data.SqlClient;

namespace APBD_cw09.Services;

public class WarehouseService : IWarehousesService
{
    private readonly IConfiguration _configuration;

    public WarehouseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        string query = @"SELECT 1 FROM Warehouse WHERE IdWarehouse = @id";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            
            await conn.OpenAsync();
            
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }
    }
}