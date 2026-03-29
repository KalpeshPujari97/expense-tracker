namespace ExpenseTracker.API.DTOs
{
    public class CreateTransactionDTO
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string? Description { get; set; }
    }

    public class TransactionResponseDTO
    {
        public int TransactionId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MonthlySummaryDTO
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public List<CategorySummaryDTO> CategoryBreakdown { get; set; } = new();
    }

    public class CategorySummaryDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
