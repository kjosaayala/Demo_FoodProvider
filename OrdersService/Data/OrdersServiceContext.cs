using Microsoft.EntityFrameworkCore;

namespace OrdersService.Data
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
        }
    }

    public class Order
        {
            public int Id { get; set; }
            public string OrderId { get; set; }
            public string CustomerId { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = "Pending";
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
}
