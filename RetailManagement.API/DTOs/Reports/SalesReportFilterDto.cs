using System.ComponentModel.DataAnnotations;

namespace RetailManagement.API.DTOs.Reports
{
    public class SalesReportFilterDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }


        [Range(1, int.MaxValue)]
        public int? ProductId { get; set; }


        [Range(1, int.MaxValue)]
        public int? CustomerId { get; set; }


        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }
    }


    public class PagedSalesReportFilterDto : SalesReportFilterDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;


        [Range(1, 100)]
        public int PageSize { get; set; } = 10;


        public string SortBy { get; set; } = "NetSales";
        public string SortDirection { get; set; } = "DESC";
    }
}