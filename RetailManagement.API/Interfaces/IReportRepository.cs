using RetailManagement.API.DTOs.Reports;

namespace RetailManagement.API.Interfaces
{
    public interface IReportRepository
    {
        Task<SalesSummaryResponseDto> GetSalesSummaryAsync(SalesReportFilterDto filter);

        Task<PagedReportResponseDto<ProductSalesResponseDto>> GetProductSalesAsync( PagedSalesReportFilterDto filter);

        Task<PagedReportResponseDto<CustomerSalesResponseDto>> GetCustomerSalesAsync( PagedSalesReportFilterDto filter);
    }
}