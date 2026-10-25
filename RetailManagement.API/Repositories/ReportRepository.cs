using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RetailManagement.API.DTOs.Reports;
using RetailManagement.API.Interfaces;

namespace RetailManagement.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IConfiguration _configuration;

        public ReportRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SalesSummaryResponseDto> GetSalesSummaryAsync(SalesReportFilterDto filter)
        {
            ValidateDates(filter);

            var parameters = BuildFilterParameters(filter);

            await using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<SalesSummaryResponseDto>(
                "dbo.sp_GetSalesSummary",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result ?? new SalesSummaryResponseDto();
        }


        public async Task<PagedReportResponseDto<ProductSalesResponseDto>> GetProductSalesAsync(
            PagedSalesReportFilterDto filter)
        {
            ValidateDates(filter);

            var parameters = BuildPagedParameters(filter);

            await using var connection = CreateConnection();

            var rows = (await connection.QueryAsync<ProductSalesRowDto>(
                "dbo.sp_GetProductSales",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


            int totalRecords = rows.FirstOrDefault()?.TotalRecords ?? 0;

            var items = rows.Cast<ProductSalesResponseDto>().ToList();

            return BuildPagedResponse(items, totalRecords, filter);
        }


        public async Task<PagedReportResponseDto<CustomerSalesResponseDto>> GetCustomerSalesAsync(
            PagedSalesReportFilterDto filter)
        {
            ValidateDates(filter);

            var parameters = BuildPagedParameters(filter);

            await using var connection = CreateConnection();

            var rows = (await connection.QueryAsync<CustomerSalesRowDto>(
                "dbo.sp_GetCustomerSales",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalRecords = rows.FirstOrDefault()?.TotalRecords ?? 0;

            var items = rows.Cast<CustomerSalesResponseDto>().ToList();

            return BuildPagedResponse(items, totalRecords, filter);
        }



        // helper methods

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        private static void ValidateDates(SalesReportFilterDto filter)
        {
            if (filter.FromDate == default || filter.ToDate == default)
            {
                throw new ArgumentException("FromDate and ToDate are required.");
            }

            if (filter.ToDate < filter.FromDate)
            {
                throw new ArgumentException("ToDate must be greater than or equal to FromDate.");
            }
        }


        private static DynamicParameters BuildFilterParameters(SalesReportFilterDto filter)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@FromDate", filter.FromDate.Date);
            parameters.Add("@ToDate", filter.ToDate.Date);
            parameters.Add("@ProductId", filter.ProductId);
            parameters.Add("@CustomerId", filter.CustomerId);
            parameters.Add("@CategoryId", filter.CategoryId);

            return parameters;
        }


        private static DynamicParameters BuildPagedParameters(PagedSalesReportFilterDto filter)
        {
            var parameters = BuildFilterParameters(filter);

            parameters.Add("@PageNumber", filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@SortBy", filter.SortBy);
            parameters.Add("@SortDirection", filter.SortDirection);

            return parameters;
        }


        private static PagedReportResponseDto<T> BuildPagedResponse<T>( List<T> items, int totalRecords, PagedSalesReportFilterDto filter)
        {
            return new PagedReportResponseDto<T>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalRecords == 0
                    ? 0
                    : (int)Math.Ceiling((double)totalRecords / filter.PageSize)
            };
        }
    }
}