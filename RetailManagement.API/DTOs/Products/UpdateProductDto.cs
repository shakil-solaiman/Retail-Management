using System.ComponentModel.DataAnnotations;

namespace RetailManagement.API.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "Product code is required.")]
        public string Code { get; set; } = string.Empty;


        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }


        [Range(0, int.MaxValue,   ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }
    }
}