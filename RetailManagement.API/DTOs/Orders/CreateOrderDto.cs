using System.ComponentModel.DataAnnotations;

namespace RetailManagement.API.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(150)]
        public string CustomerName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Order must contain items.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}