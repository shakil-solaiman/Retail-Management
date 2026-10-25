using RetailManagement.API.DTOs.StockTransfers;

namespace RetailManagement.API.Interfaces
{
    public interface IStockTransferRepository
    {
        Task<StockTransferResponseDto> CreateTransferAsync( CreateStockTransferDto request);
    }
}