namespace APBD_cw09.Services;

public interface IProductsService
{
    public Task<bool> DoesProductExist(int productId);
    public Task<double> GetProductPrice(int productId);
}