using System.ComponentModel.DataAnnotations;

namespace APBD_cw09.Models.DTOs;

public class ProductWarehouseDTO
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Amount { get; set; }
    
    [Required]
    public string CreatedAt { get; set; }
}