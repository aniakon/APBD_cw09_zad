using System.Data.Common;
using APBD_cw09.Models.DTOs;
using APBD_cw09.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw09.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private IProductsService _productsService;
    private IWarehousesService _warehousesService;
    private IOrdersService _ordersService;
    private IProductWarehousesService _productWarehousesService;
    
    public WarehouseController(IProductsService productsService, IWarehousesService warehousesService, IOrdersService ordersService, IProductWarehousesService productWarehousesService)
    {
        _productsService = productsService;
        _warehousesService = warehousesService;
        _ordersService = ordersService;
        _productWarehousesService = productWarehousesService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(ProductWarehouseDTO product)
    {
        // checks if product of given index exists
        if (!await _productsService.DoesProductExist(product.IdProduct))
        {
            return NotFound("Produkt o danym id nie istnieje w bazie.");
        }
        
        // checks if warehouse of given index exists
        if (!await _warehousesService.DoesWarehouseExist(product.IdWarehouse))
        {
            return NotFound("Warehouse o danym id nie istnieje w bazie.");
        }

        // checks if an order of the product with the given amount exists in table Order
        int? orderIdNullable = await _ordersService.GetOrderId(product.IdProduct, product.Amount);
        if (orderIdNullable == null)
        {
            return NotFound("Nie ma w bazie zamówienia danego produktu z zadaną ilością.");
        }

        // checks if given date is later than the date of order creation
        if (!await _ordersService.IsOrderDateEarlier(product.IdProduct, product.Amount, product.CreatedAt))
        {
            return BadRequest(
                "Data utworzenia w żądaniu nie może być wcześniejsza niż data utworzenia zamówienia w bazie.");
        }
        
        // checks if the order of given product and amount was already completed (in table Product_Warehouse)
        if (! await _productWarehousesService.WasOrderCompleted(product.IdProduct, product.Amount))
        {
            return BadRequest("To zamowienie już zostało zrealizowane.");
        }

        // updated the time (FullfilledAt) of the order
        if (!await _ordersService.UpdateDate(orderIdNullable.Value))
        {
            return StatusCode(500, "Problem ze zaktualizowaniem daty zamowienia FullfilledAt.");
        }
        
        // inserts the order into Product_Warehouse table and return given id
        var res = await _productWarehousesService.InsertProductWarehouse(product.IdProduct, product.IdWarehouse, product.Amount);
        if (res != null) return Ok(res);
        return StatusCode(500, "Problem ze wstawieniem danych do ProductWarehouse.");
    }

    [HttpPost("proc")]
    public async Task<ActionResult<int>> AddProductToWarehouseProc(ProductWarehouseDTO product)
    {
        try
        {
            var res = await _productWarehousesService.AddProductWarehouseAsync(product);
            return Ok(res);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}