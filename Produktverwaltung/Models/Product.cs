namespace Produktverwaltung.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? UniqueIdentifier { get; set; }
        public string? filename { get; set; }
    }
}
