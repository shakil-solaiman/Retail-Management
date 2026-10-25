namespace RetailManagement.API.DTOs.StockTransfers
{
    public class StockTransferResponseDto
    {
        public int Id { get; set; }

        public int FromWarehouseId { get; set; }

        public int ToWarehouseId { get; set; }

        public DateTime TransferDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<StockTransferItemResponseDto> Items { get; set; } = new();
    }
}