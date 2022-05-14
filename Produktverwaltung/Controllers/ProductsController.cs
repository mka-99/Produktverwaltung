using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Produktverwaltung.Helper;
using Produktverwaltung.Models;
using Produktverwaltung.Services;
using System.Drawing;

namespace Produktverwaltung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ProductContext context,
            ILogger<ProductsController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.TodoItems.FindAsync(id);

            _logger.LogInformation($"...searching for {id}");

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            _logger.LogInformation($"...searching for {id}");

            if (id != product.Id)
            {
                return BadRequest();
            }

            var product1 = await _context.TodoItems.FindAsync(id);
            if (product1 == null)
            {
                return NotFound();
            }

            product1.Name = product.Name;
            product1.Price = product.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ProductExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {


            _context.TodoItems.Add(product);
            product.UniqueIdentifier = product.Id + "_" + _configuration["Suffix"];

            await _context.SaveChangesAsync();

            _logger.LogInformation($"saved entry: {product.Id}");
            _logger.LogInformation($"Bezeichnung: {product.UniqueIdentifier}");


            //return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _context.TodoItems.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"...searching for {id}");

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static Product ItemToDTO(Product product) =>
    new Product
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        UniqueIdentifier = product.UniqueIdentifier,
        filename = product.filename,
    };
    }
}
