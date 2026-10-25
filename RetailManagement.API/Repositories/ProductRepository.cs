using Microsoft.EntityFrameworkCore;
using RetailManagement.API.Data;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Models;

namespace RetailManagement.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly RetailDbContext _context;

        public ProductRepository(RetailDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return product;
        }


        public async Task<List<Product>> GetAllAsync(string? search, int pageNumber, int pageSize)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.IsActive);


            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(p => p.Name.Contains(search) || p.Code.Contains(search));
            }

            return await query.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }


        public async Task<int> GetTotalCountAsync(string? search)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(p => p.Name.Contains(search) || p.Code.Contains(search));
            }

            return await query.CountAsync();
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeProductId = null)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.Code == code);

            if (excludeProductId.HasValue)
            {
                query = query.Where(p => p.Id != excludeProductId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            _context.Products.Update(product);

            int savedRows = await _context.SaveChangesAsync();

            return savedRows > 0;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return false;
            }

            product.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}