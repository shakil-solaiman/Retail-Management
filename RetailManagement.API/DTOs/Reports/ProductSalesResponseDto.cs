namespace RetailManagement.API.DTOs.Reports
{
    public class ProductSalesResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int TotalQuantity { get; set; }

        public decimal GrossSales { get; set; }

        public decimal Discount { get; set; }

        public decimal NetSales { get; set; }
    }


    public class ProductSalesRowDto : ProductSalesResponseDto
    {
        public int TotalRecords { get; set; }
    }
}