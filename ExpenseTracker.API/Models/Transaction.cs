namespace ExpenseTracker.API.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Category? Category { get; set; }

    }
}
