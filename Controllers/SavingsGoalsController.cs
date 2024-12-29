using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Data;
using SmartFlow.Models;

namespace SmartFlow.Controllers
{
    public class SavingsGoalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavingsGoalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SavingsGoals
        public async Task<IActionResult> Index()
        {
            return View(await _context.SavingsGoal.Include(m => m.User).Where(m => m.userId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync());
        }

        // GET: SavingsGoals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsGoal = await _context.SavingsGoal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savingsGoal == null)
            {
                return NotFound();
            }

            return View(savingsGoal);
        }

        // GET: SavingsGoals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SavingsGoals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GoalName,TargetAmount,CurrentAmount,TargetDate")] SavingsGoal savingsGoal)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    savingsGoal.userId = userId;
                }
                _context.Add(savingsGoal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(savingsGoal);
        }

        // GET: SavingsGoals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsGoal = await _context.SavingsGoal.Where(t => t.userId == User.FindFirstValue(ClaimTypes.NameIdentifier)).FirstOrDefaultAsync(s => s.Id == id);
            if (savingsGoal == null)
            {
                return NotFound();
            }
            return View(savingsGoal);
        }

        // POST: SavingsGoals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GoalName,TargetAmount,TargetDate")] SavingsGoal savingsGoal)
        {
            if (id != savingsGoal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (userId == null)
                    {
                        return Unauthorized();
                    }

                    // Pobierz istniejący cel oszczędnościowy dla zalogowanego użytkownika
                    var existingGoal = await _context.SavingsGoal
                        .Where(g => g.userId == userId)
                        .FirstOrDefaultAsync(g => g.Id == id);

                    if (existingGoal == null)
                    {
                        return NotFound();
                    }

                    // Aktualizuj pola, które mogą być zmieniane przez użytkownika
                    existingGoal.GoalName = savingsGoal.GoalName;
                    existingGoal.TargetAmount = savingsGoal.TargetAmount;
                    existingGoal.TargetDate = savingsGoal.TargetDate;

                    _context.Update(existingGoal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingsGoalExists(savingsGoal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(savingsGoal);
        }

        // GET: SavingsGoals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsGoal = await _context.SavingsGoal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savingsGoal == null)
            {
                return NotFound();
            }

            return View(savingsGoal);
        }

        // POST: SavingsGoals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savingsGoal = await _context.SavingsGoal.FindAsync(id);
            if (savingsGoal != null)
            {
                _context.SavingsGoal.Remove(savingsGoal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingsGoalExists(int id)
        {
            return _context.SavingsGoal.Any(e => e.Id == id);
        }
    }
}
