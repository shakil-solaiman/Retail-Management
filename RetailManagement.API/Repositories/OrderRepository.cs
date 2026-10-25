using Microsoft.EntityFrameworkCore;
using System.Data;
using RetailManagement.API.Data;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Models;

namespace RetailManagement.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RetailDbContext _context;

        public OrderRepository(RetailDbContext context)
        {
            _context = context;
        }


        // Create Order
        public async Task<Order> CreateOrderAsync(Order order)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                if (order.OrderItems.Count == 0)
                {
                    throw new ArgumentException( "Order must contain at least one item.");
                }



                // Check duplicate products
                var productIds = order.OrderItems.Select(i => i.ProductId).ToList();

                if (productIds.Count != productIds.Distinct().Count())
                {
                    throw new ArgumentException("Duplicate products are not allowed.");
                }

                decimal totalAmount = 0;


                foreach (var item in order.OrderItems)
                {
                    // Validate quantity
                    if (item.Quantity <= 0)
                    {
                        throw new ArgumentException( "Quantity must be greater than zero.");
                    }


                    // Find product
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && p.IsActive);

                    if (product == null)
                    {
                        throw new ArgumentException($"Product {item.ProductId} not found.");
                    }


                    // Check stock
                    if (product.Stock < item.Quantity)
                    {
                        throw new ArgumentException($"Insufficient stock for product {product.Name}.");
                    }


                    // Calculate prices
                    item.UnitPrice = product.Price;

                    item.TotalPrice = item.UnitPrice * item.Quantity;

                    totalAmount += item.TotalPrice;


                    // change
                    var warehouseStock = await _context.WarehouseStocks.FirstOrDefaultAsync(ws => ws.WarehouseId == 1 &&ws.ProductId == item.ProductId);

                    if (warehouseStock == null ||
                        warehouseStock.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient warehouse stock for product {item.ProductId}.");
                    }

                    warehouseStock.Quantity -= item.Quantity;

                    // change

                    // Decrease stock
                    product.Stock -= item.Quantity;
                }



                order.CustomerName = order.CustomerName.Trim();
                order.TotalAmount = totalAmount;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // Get Order by ID
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderItems).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }



        // Get All Orders
        public async Task<List<Order>> GetAllAsync(
            string? customerName,
            string? status,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Orders.Include(o => o.OrderItems).AsNoTracking().AsQueryable();


            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(o =>o.CustomerName.Contains(customerName.Trim()));
            }


            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status.Trim());
            }


            return await query
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        // Get Total Order Count
        public async Task<int> GetTotalCountAsync(string? customerName, string? status)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(o => o.CustomerName.Contains(customerName.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status.Trim());
            }

            return await query.CountAsync();
        }



        // Cancel Order
        public async Task<bool> CancelOrderAsync(int id)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var order = await _context.Orders.Include(o => o.OrderItems) .FirstOrDefaultAsync(o => o.Id == id);


                if (order == null)
                {
                    return false;
                }


                if (order.Status == "Cancelled")
                {
                    throw new InvalidOperationException( "Order is already cancelled.");
                }


                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product {item.ProductId} not found.");
                    }


                    // change
                    var warehouseStock = await _context.WarehouseStocks.FirstOrDefaultAsync(ws => ws.WarehouseId == 1 && ws.ProductId == item.ProductId);

                    if (warehouseStock == null)
                    {
                        throw new InvalidOperationException( $"Warehouse stock record not found for product {item.ProductId}.");
                    }


                    warehouseStock.Quantity += item.Quantity;

                    //change


                    // Restore stock
                    product.Stock += item.Quantity;
                }

                order.Status = "Cancelled";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}