using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.API.Models;


namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> AddTransaction(CreateTransactionDTO dto)
        {
            var transaction = new Transaction
            {
                UserId = GetUserId(),
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                TransactionDate = dto.TransactionDate,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok("Transaction added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var userId = GetUserId();
            var currentDate = DateTime.UtcNow;

            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (month.HasValue && year.HasValue)
            {
                query = query.Where(t =>
                    t.TransactionDate.Month == month.Value &&
                    t.TransactionDate.Year == year.Value);
            }
            else
            {
                query = query.Where(t =>
                    t.TransactionDate.Month == currentDate.Month &&
                    t.TransactionDate.Year == currentDate.Year);
            }

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionResponseDTO
                {
                    TransactionId = t.TransactionId,
                    CategoryName = t.Category!.Name,
                    CategoryType = t.Category.Type,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = GetUserId();
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);

            if (transaction == null)
                return NotFound("Transaction not found.");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok("Transaction deleted successfully.");
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetMonthlySummary(
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var userId = GetUserId();
            var currentDate = DateTime.UtcNow;
            var targetMonth = month ?? currentDate.Month;
            var targetYear = year ?? currentDate.Year;

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId &&
                       t.TransactionDate.Month == targetMonth &&
                       t.TransactionDate.Year == targetYear)
                .ToListAsync();

            var summary = new MonthlySummaryDTO
            {
                TotalIncome = transactions
                    .Where(t => t.Category!.Type == "Income")
                    .Sum(t => t.Amount),
                TotalExpense = transactions
                    .Where(t => t.Category!.Type == "Expense")
                    .Sum(t => t.Amount),
                CategoryBreakdown = transactions
                    .GroupBy(t => new { t.Category!.Name, t.Category.Type })
                    .Select(g => new CategorySummaryDTO
                    {
                        CategoryName = g.Key.Name,
                        CategoryType = g.Key.Type,
                        Total = g.Sum(t => t.Amount)
                    })
                    .OrderByDescending(c => c.Total)
                    .ToList()
            };

            summary.Balance = summary.TotalIncome - summary.TotalExpense;

            return Ok(summary);
        }
    }
}
