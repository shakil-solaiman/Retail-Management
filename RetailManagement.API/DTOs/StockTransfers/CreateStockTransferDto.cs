using System.ComponentModel.DataAnnotations;

namespace RetailManagement.API.DTOs.StockTransfers
{
    public class CreateStockTransferDto
    {
        [Range(1, int.MaxValue)]
        public int FromWarehouseId { get; set; }


        [Range(1, int.MaxValue)]
        public int ToWarehouseId { get; set; }


        [Required] [MinLength(1)]
        public List<CreateStockTransferItemDto> Items { get; set; } = new();
    }
}