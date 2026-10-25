using System.Data;
using Microsoft.EntityFrameworkCore;
using RetailManagement.API.Data;
using RetailManagement.API.DTOs.StockTransfers;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Models;

namespace RetailManagement.API.Repositories
{
    public class StockTransferRepository : IStockTransferRepository
    {
        private readonly RetailDbContext _context;

        public StockTransferRepository(RetailDbContext context)
        {
            _context = context;
        }


        public async Task<StockTransferResponseDto> CreateTransferAsync(CreateStockTransferDto request)
        {
           
            if (request.FromWarehouseId == request.ToWarehouseId)
            {
                throw new ArgumentException("Source and destination warehouses must be different.");
            }


            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one product is required.");
            }

            if (request.Items.Any(x => x.Quantity <= 0 || x.ProductId <= 0))
            {
                throw new ArgumentException("Product IDs and quantities must be greater than zero.");
            }

           

            if (request.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Duplicate products are not allowed.");
            }



            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
              
                var fromWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.FromWarehouseId && w.IsActive);

                if (fromWarehouse == null)
                {
                    throw new ArgumentException("Source warehouse does not exist or is inactive.");
                }


                var toWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.ToWarehouseId && w.IsActive);

                if (toWarehouse == null)
                {
                    throw new ArgumentException("Destination warehouse does not exist or is inactive.");
                }

                var productIds = request.Items.Select(x => x.ProductId).ToList();


                var activeProductIds = await _context.Products.Where(p => productIds.Contains(p.Id) && p.IsActive).Select(p => p.Id).ToListAsync();

                if (activeProductIds.Count != productIds.Count)
                {
                    throw new ArgumentException("One or more products do not exist or are inactive.");
                }




                // Load warehouse stocks 
                var stocks = await _context.WarehouseStocks.Where(s => (s.WarehouseId == request.FromWarehouseId || s.WarehouseId == request.ToWarehouseId) &&
                     productIds.Contains(s.ProductId))
                    .OrderBy(s => s.WarehouseId)
                    .ThenBy(s => s.ProductId)
                    .ToListAsync();




                // Validate quantities 
                foreach (var item in request.Items)
                {
                    var sourceStock = stocks.FirstOrDefault(s => s.WarehouseId == request.FromWarehouseId && s.ProductId == item.ProductId);

                    if (sourceStock == null || sourceStock.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
                    }
                }




                // transfer header
                var transfer = new StockTransfer
                {
                    FromWarehouseId = request.FromWarehouseId,
                    ToWarehouseId = request.ToWarehouseId,
                    TransferDate = DateTime.UtcNow,
                    Status = "Completed"
                };




                _context.StockTransfers.Add(transfer);


                // Update warehouse stocks
                foreach (var item in request.Items)
                {
                    var sourceStock = stocks.First(s => s.WarehouseId == request.FromWarehouseId && s.ProductId == item.ProductId);

                    sourceStock.Quantity -= item.Quantity;

                    var destinationStock = stocks.FirstOrDefault(s =>  s.WarehouseId == request.ToWarehouseId && s.ProductId == item.ProductId);

                    if (destinationStock == null)
                    {
                        destinationStock = new WarehouseStock
                        {
                            WarehouseId = request.ToWarehouseId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        };

                        _context.WarehouseStocks.Add(destinationStock);
                    }
                    else
                    {
                        destinationStock.Quantity += item.Quantity;
                    }



                    _context.StockTransferItems.Add(new StockTransferItem
                        {
                            StockTransfer = transfer,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                }



                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new StockTransferResponseDto
                {
                    Id = transfer.Id,
                    FromWarehouseId = transfer.FromWarehouseId,
                    ToWarehouseId = transfer.ToWarehouseId,
                    TransferDate = transfer.TransferDate,
                    Status = transfer.Status,

                    Items = request.Items.Select(item => new StockTransferItemResponseDto
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}