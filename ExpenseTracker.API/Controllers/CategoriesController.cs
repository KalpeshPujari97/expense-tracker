using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetCategoriesByType(string type)
        {
            var categories = await _context.Categories
                .Where(c => c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(categories);
        }
    }
}
