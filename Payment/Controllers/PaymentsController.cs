
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment.Data;
using static Payment.Data.PaymentContext;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentContext _context;
        public PaymentsController(PaymentContext context)
        {
            _context = context;
        }

        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == request.CustomerId);

            if (customer == null)
                return NotFound("Customer not found.");

            customer.Balance -= request.Amount;

            var transaction = new Transaction
            {
                OrderId = request.OrderId.ToString(),
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Status = "Success"
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new
            {
                transaction.OrderId,
                transaction.Status,
                RemainingBalance = customer.Balance
            });
        }

        [HttpGet("transactions")]
        public IActionResult GetTransactions()
        {
            var transactions = _context.Transactions.ToList();
            return Ok(transactions);
        }

        [HttpGet("customers")]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }
    }

    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
