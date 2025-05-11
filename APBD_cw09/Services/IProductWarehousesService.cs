using APBD_cw09.Models.DTOs;

namespace APBD_cw09.Services;

public interface IProductWarehousesService
{
    public Task<bool> WasOrderCompleted(int idProduct, int amount);
    public Task<int?> InsertProductWarehouse(int idProduct, int idWarehouse, int amount);
    public Task<int> AddProductWarehouseAsync(ProductWarehouseDTO product);
}