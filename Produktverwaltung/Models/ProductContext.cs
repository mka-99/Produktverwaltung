using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Produktverwaltung.Models
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options)
           : base(options)
        {
        }

        public DbSet<Product> TodoItems { get; set; } = null!;
    }
}
