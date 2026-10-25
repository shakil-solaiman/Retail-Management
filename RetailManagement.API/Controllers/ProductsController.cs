using Microsoft.AspNetCore.Mvc;
using RetailManagement.API.DTOs.Products;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Models;

namespace RetailManagement.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(
            CreateProductDto createProductDto)
        {
            string name = createProductDto.Name.Trim();
            string code = createProductDto.Code.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    message = "Product name is required."
                });
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new
                {
                    message = "Product code is required."
                });
            }

            bool codeExists = await _productRepository.CodeExistsAsync(code);

            if (codeExists)
            {
                return BadRequest(new
                {
                    message = "Product code already exists."
                });
            }

            Product product = new Product
            {
                Name = name,
                Code = code,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            Product createdProduct = await _productRepository.CreateAsync(product);

            ProductResponseDto response = new ProductResponseDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Code = createdProduct.Code,
                Price = createdProduct.Price,
                Stock = createdProduct.Stock,
                IsActive = createdProduct.IsActive,
                CreatedAt = createdProduct.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = response.Id },
                response);
        }



        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
        {
            Product? product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            ProductResponseDto response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };

            return Ok(response);
        }




        [HttpGet]
        public async Task<IActionResult> GetAllProducts( [FromQuery] string? search, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(new
                {
                    message = "Page number must be greater than zero."
                });
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    message = "Page size must be between 1 and 100."
                });
            }

            var products = await _productRepository.GetAllAsync(
                search, pageNumber, pageSize);

            int totalCount = await _productRepository.GetTotalCountAsync(search);

            var productDtos = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                data = productDtos
            });
        }




        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            // Find the existing product
            Product? product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }


            // Remove unnecessary spaces
            string name = updateProductDto.Name.Trim();
            string code = updateProductDto.Code.Trim();


            // Validate name
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    message = "Product name is required."
                });
            }


            // Validate code
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new
                {
                    message = "Product code is required."
                });
            }


            // Check duplicate product code
            bool codeExists = await _productRepository.CodeExistsAsync(code, id);

            if (codeExists)
            {
                return BadRequest(new
                {
                    message = "Product code already exists."
                });
            }


            // Update product information
            product.Name = name;
            product.Code = code;
            product.Price = updateProductDto.Price;
            product.Stock = updateProductDto.Stock;

            // Save changes
            await _productRepository.UpdateAsync(product);

            return NoContent();
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            bool deleted = await _productRepository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            return NoContent();
        }




    }
}