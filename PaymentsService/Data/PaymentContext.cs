using Microsoft.EntityFrameworkCore;

namespace PaymentsService.Data
{
    public class PaymentContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public PaymentContext(DbContextOptions<PaymentContext> options)
            : base(options)
        {
        }

        public class Transaction
        {
            public int Id { get; set; }
            public string OrderId { get; set; }
            public string CustomerId { get; set; }
            public double Amount { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class Customer
        {
            public int Id { get; set; }
            public string CustomerId { get; set; }
            public string Name { get; set; }
            public double Balance { get; set; }
        }
    }
}
