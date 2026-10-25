namespace RetailManagement.API.DTOs.Reports
{
    public class SalesSummaryResponseDto
    {
        public int TotalInvoices { get; set; }

        public int TotalQuantity { get; set; }

        public decimal GrossSales { get; set; }

        public decimal Discount { get; set; }

        public decimal NetSales { get; set; }
    }
}