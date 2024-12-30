using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Data;
using SmartFlow.Models;
using System.Security.Claims;

namespace SmartFlow.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.SavingsGoal)
                .Where(t => t.userId == userId)
                .ToListAsync();

            return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.SavingsGoal)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var userId = GetUserId();

            ViewBag.CategoryTitleWithIcon = _context.Categories
                .Where(c => c.userId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Title
                }).ToList();

            ViewBag.SavingsGoalsNames = _context.SavingsGoal
                .Where(s => s.userId == userId)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.GoalName
                }).ToList();

            return View(new Transaction { Date = DateTime.Now });
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Amount,Note,Date,Type,SavingsGoalId")] Transaction transaction)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryTitleWithIcon = _context.Categories
                    .Where(c => c.userId == userId)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Title
                    }).ToList();

                ViewBag.SavingsGoalsNames = _context.SavingsGoal
                    .Where(s => s.userId == userId)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.GoalName
                    }).ToList();

                return View(transaction);
            }

            try
            {
                transaction.userId = userId;
                _context.Add(transaction);

                if (transaction.SavingsGoalId.HasValue)
                {
                    var savingsGoal = await _context.SavingsGoal.FindAsync(transaction.SavingsGoalId.Value);
                    if (savingsGoal != null)
                    {
                        savingsGoal.CurrentAmount += transaction.Amount;
                        _context.Update(savingsGoal);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the transaction.");
            }

            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            var userId = GetUserId();

            ViewData["CategoryTitleWithIcon"] = _context.Categories
                .Where(c => c.userId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Title
                }).ToList();

            ViewData["SavingsGoalList"] = _context.SavingsGoal
                .Where(s => s.userId == userId)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.GoalName
                }).ToList();

            return View(transaction);
        }


        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Amount,Note,Date,Type,SavingsGoalId")] Transaction transaction)
        {
            if (id != transaction.Id) return NotFound();

            if (!ModelState.IsValid) return View(transaction);

            try
            {
                var userId = GetUserId();
                transaction.userId = userId;

                var originalTransaction = await _context.Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && t.userId == userId);

                if (originalTransaction == null) return NotFound();

                if (originalTransaction.SavingsGoalId.HasValue)
                {
                    var originalSavingsGoal = await _context.SavingsGoal.FindAsync(originalTransaction.SavingsGoalId.Value);
                    if (originalSavingsGoal != null)
                    {
                        originalSavingsGoal.CurrentAmount -= originalTransaction.Amount;
                        _context.Update(originalSavingsGoal);
                    }
                }

                if (transaction.SavingsGoalId.HasValue)
                {
                    var newSavingsGoal = await _context.SavingsGoal.FindAsync(transaction.SavingsGoalId.Value);
                    if (newSavingsGoal != null)
                    {
                        newSavingsGoal.CurrentAmount += transaction.Amount;
                        _context.Update(newSavingsGoal);
                    }
                }

                // Zapisz zmiany dla transakcji
                _context.Update(transaction);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.Id == transaction.Id)) return NotFound();
                throw;
            }
        }


        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.SavingsGoal)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.SavingsGoal)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound();

            if (transaction.SavingsGoalId.HasValue && transaction.SavingsGoal != null)
            {
                transaction.SavingsGoal.CurrentAmount -= transaction.Amount;
                _context.Update(transaction.SavingsGoal);
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
