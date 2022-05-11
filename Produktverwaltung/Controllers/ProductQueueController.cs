#nullable disable
using System.Configuration;
using System.Drawing;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produktverwaltung.Helper;
using Produktverwaltung.Models;
using Produktverwaltung.Services;

namespace Produktverwaltung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductQueueController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductsController> _logger;

        public ProductQueueController(
            ProductContext context,
            ILogger<ProductsController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/ProductQueue
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/ProductQueue/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.TodoItems.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/ProductQueue/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductQueue
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.TodoItems.Add(product);
            await _context.SaveChangesAsync();


            //dummy bitmap
            Bitmap img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            // The font for our text
            Font f = new Font("Arial", 14);

            // work out how big the text will be when drawn as an image
            SizeF size = drawing.MeasureString(product.Name, f);

            // create a new Bitmap of the required size
            img = new Bitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
            drawing = Graphics.FromImage(img);

            // give it a white background
            drawing.Clear(Color.White);

            // draw the text in black
            drawing.DrawString(product.Name, f, Brushes.Black, 0, 0);

            img.Save(@$".\{product.Name}.jpg");
            drawing.Save();

            BlobService blobService = new BlobService(_configuration);

            QueueService queueService = new QueueService(_configuration);

            blobService.UploadDataToBlobContainer(Environment.CurrentDirectory, product.Name, "imageblob");
            queueService.CreateQueue("productqueue");
            queueService.InsertMessage("productqueue", Newtonsoft.Json.JsonConvert.SerializeObject(product));
        

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductQueue/5
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

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
