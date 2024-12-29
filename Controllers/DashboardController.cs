using SmartFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Data;
using System.Security.Claims;

namespace SmartFlow
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            // Last 7 Days Transactions
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(t => t.Category) // Załaduj kategorię
                .Where(y => (y.Date >= StartDate && y.Date <= EndDate) && y.userId == userId)
                .ToListAsync();

            // Calculate Total Income Last 7 Days
            int TotalIncome = await _context.Transactions
                .Where(i => i.Type == "Income" && i.userId == userId) // Bezpośrednio z Transaction
                .SumAsync(i => i.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            // Calculate Total Expense Last 7 Days
            int TotalExpense = await _context.Transactions
                .Where(i => i.Type == "Expense" && i.userId == userId) // Bezpośrednio z Transaction
                .SumAsync(i => i.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            // Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");

            // Recent 5 Transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category) // Załaduj kategorię
                .Where(i => i.userId == userId)
                .OrderByDescending(i => i.Date)
                .Take(5)
                .ToListAsync();

            // Add Savings Goals to the Dashboard
            var savingsGoals = await _context.SavingsGoal.Where(s => s.userId == userId).ToListAsync();
            ViewBag.SavingsGoals = savingsGoals;

            return View();
        }
    }
}
