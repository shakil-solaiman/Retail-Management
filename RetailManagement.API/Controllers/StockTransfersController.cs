using Microsoft.AspNetCore.Mvc;
using RetailManagement.API.DTOs.StockTransfers;
using RetailManagement.API.Interfaces;

namespace RetailManagement.API.Controllers
{
    [Route("api/stock-transfers")]
    [ApiController]
    public class StockTransfersController : ControllerBase
    {
        private readonly IStockTransferRepository _stockTransferRepository;

        public StockTransfersController(
            IStockTransferRepository stockTransferRepository)
        {
            _stockTransferRepository = stockTransferRepository;
        }



        // POST: api/stock-transfers
        [HttpPost]
        public async Task<IActionResult> CreateTransfer([FromBody] CreateStockTransferDto request)
        {
            try
            {
                var result = await _stockTransferRepository
                    .CreateTransferAsync(request);

                return StatusCode(201, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}