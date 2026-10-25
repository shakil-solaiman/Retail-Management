using Microsoft.AspNetCore.Mvc;
using RetailManagement.API.DTOs.Orders;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Models;

namespace RetailManagement.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                return BadRequest(new
                {
                    message = "Customer name is required."
                });
            }

            var order = new Order
            {
                CustomerName = dto.CustomerName.Trim(),
                OrderItems = dto.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            try
            {
                var createdOrder = await _orderRepository.CreateOrderAsync(order);

                var response = MapToResponseDto(createdOrder);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = response.Id },
                    response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }



        // GET: api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            return Ok(MapToResponseDto(order));
        }



        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? customerName,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    message = "Invalid pagination values."
                });
            }

            var orders = await _orderRepository.GetAllAsync(
                customerName, status, pageNumber, pageSize);

            int totalCount = await _orderRepository.GetTotalCountAsync(
                customerName, status);

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                data = orders.Select(MapToResponseDto).ToList()
            });
        }



        // PATCH: api/orders/{id}/cancel
        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                bool cancelled = await _orderRepository.CancelOrderAsync(id);

                if (!cancelled)
                {
                    return NotFound(new
                    {
                        message = "Order not found."
                    });
                }

                return Ok(new
                {
                    message = "Order cancelled successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }



        // Reusable DTO mapping
        private static OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }
    }
}