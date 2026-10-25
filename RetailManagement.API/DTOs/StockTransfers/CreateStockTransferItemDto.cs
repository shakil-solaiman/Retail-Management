using System.ComponentModel.DataAnnotations;

namespace RetailManagement.API.DTOs.StockTransfers
{
    public class CreateStockTransferItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }


        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}