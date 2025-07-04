using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Model;
using System.Security.Claims;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;

            if (!_context.Products.Any())
            {
                _context.Products.AddRange(
                    new Product { Name = "Laptop", Price = 1200, Quantity = 200 },
                    new Product { Name = "Mouse", Price = 25, Quantity = 300 },
                    new Product { Name = "Keyboard", Price = 50, Quantity = 200 }
                );
                _context.SaveChanges();
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound(new { error = "Product not found" });

            return Ok(product);
        }



        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest(new { error = "Name field is required" }); 

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest(new { error = "Product ID mismatch" });

            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return NotFound(new { error = "Product not found" });

            existing.Name = product.Name;
            existing.Price = product.Price;

            await _context.SaveChangesAsync();
            return Ok(existing); 
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { error = "Product not found" }); 

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        // Authorization 
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(new { message = $"Welcome User {userId}" });
        }


        // Role
        [Authorize(Roles = "admin")]
        [HttpGet("admin/users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_context.Users.ToList());
        }

    }
}
