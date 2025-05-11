namespace APBD_cw09.Services;

public interface IWarehousesService
{
    public Task<bool> DoesWarehouseExist(int id);
}