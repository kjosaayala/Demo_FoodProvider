using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class InventoryContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options)
        {

        }

        public class Product
        {
            public int Id { get; set; }
            public string ProductId { get; set; }
            public string Name { get; set; }
            public int Stock { get; set; }
            public decimal Price { get; set; }
        }
    }
}
