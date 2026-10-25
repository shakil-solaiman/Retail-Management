using Microsoft.AspNetCore.Mvc;
using RetailManagement.API.DTOs.Reports;
using RetailManagement.API.Interfaces;

namespace RetailManagement.API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }



        // GET: api/reports/sales-summary
        [HttpGet("sales-summary")]
        public async Task<IActionResult> GetSalesSummary([FromQuery] SalesReportFilterDto filter)
        {
            try
            {
                var result = await _reportRepository.GetSalesSummaryAsync(filter);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // GET: api/reports/product-sales
        [HttpGet("product-sales")]
        public async Task<IActionResult> GetProductSales([FromQuery] PagedSalesReportFilterDto filter)
        {
            try
            {
                var result = await _reportRepository.GetProductSalesAsync(filter);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        // GET: api/reports/customer-sales
        [HttpGet("customer-sales")]
        public async Task<IActionResult> GetCustomerSales([FromQuery] PagedSalesReportFilterDto filter)
        {
            try
            {
                var result = await _reportRepository.GetCustomerSalesAsync(filter);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}