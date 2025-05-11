using System.Data;
using System.Data.Common;
using APBD_cw09.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_cw09.Services;

public class ProductWarehousesService : IProductWarehousesService
{
    private readonly IConfiguration _configuration;
    private readonly IOrdersService _ordersService;
    private readonly IProductsService _productsService;

    public ProductWarehousesService(IConfiguration config, IOrdersService ordersService, IProductsService productsService)
    {
        _configuration = config;
        _ordersService = ordersService;
        _productsService = productsService;
    }

    public async Task<bool> WasOrderCompleted(int idProduct, int amount)
    {
        string query = @"SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            int? idOrder = await _ordersService.GetOrderId(idProduct, amount);
            if (!idOrder.HasValue) return false;
            cmd.Parameters.AddWithValue("@IdOrder", idOrder.Value);
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }
    }

    public async Task<int?> InsertProductWarehouse(int idProduct, int idWarehouse, int amount)
    {
        string query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); 
                       SELECT SCOPE_IDENTITY();";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            int? idOrder = await _ordersService.GetOrderId(idProduct, amount);
            if (!idOrder.HasValue) return null;
            double cost = await _productsService.GetProductPrice(idProduct);
            
            cmd.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            cmd.Parameters.AddWithValue("@IdProduct", idProduct);
            cmd.Parameters.AddWithValue("@IdOrder", idOrder.Value);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@Price", amount*cost);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            
            await conn.OpenAsync();
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }
    }

    public async Task<int> AddProductWarehouseAsync(ProductWarehouseDTO product)
    {
        int result;
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        
        try
        {

            command.CommandText = "AddProductToWarehouse";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", product.Amount);
            command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);

            var res  = await command.ExecuteScalarAsync();
            result = Convert.ToInt32(res);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        return result;
    }
    
}