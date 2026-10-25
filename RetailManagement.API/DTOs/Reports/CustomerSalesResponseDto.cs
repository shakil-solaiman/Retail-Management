namespace RetailManagement.API.DTOs.Reports
{
    public class CustomerSalesResponseDto
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int TotalInvoices { get; set; }

        public int TotalQuantity { get; set; }

        public decimal GrossSales { get; set; }

        public decimal Discount { get; set; }

        public decimal NetSales { get; set; }
    }



    public class CustomerSalesRowDto : CustomerSalesResponseDto
    {
        public int TotalRecords { get; set; }
    }
}