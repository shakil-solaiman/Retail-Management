namespace RetailManagement.API.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}