namespace Order.Models
{
    public class OrderRequest
    {
        public string CustomerId { get; set; }
        public int ProductId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
