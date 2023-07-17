using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JV_Imprest_Payment.Data;
using JV_Imprest_Payment.Models;

namespace JV_Imprest_Payment.Controllers
{
    public class AfeStructuresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AfeStructuresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AfeStructures
        public async Task<IActionResult> Index()
        {
              return _context.AfeStructure != null ? 
                          View(await _context.AfeStructure.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.AfeStructure'  is null.");
        }

        // GET: AfeStructures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AfeStructure == null)
            {
                return NotFound();
            }

            var afeStructure = await _context.AfeStructure
                .FirstOrDefaultAsync(m => m.Id == id);
            if (afeStructure == null)
            {
                return NotFound();
            }

            return View(afeStructure);
        }

        // GET: AfeStructures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AfeStructures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AfeCode,Classification,AfeOwner,OwnerEmail,StartDate,EndDate")] AfeStructure afeStructure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(afeStructure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(afeStructure);
        }

        // GET: AfeStructures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AfeStructure == null)
            {
                return NotFound();
            }

            var afeStructure = await _context.AfeStructure.FindAsync(id);
            if (afeStructure == null)
            {
                return NotFound();
            }
            return View(afeStructure);
        }

        // POST: AfeStructures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AfeCode,Classification,AfeOwner,OwnerEmail,StartDate,EndDate")] AfeStructure afeStructure)
        {
            if (id != afeStructure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(afeStructure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AfeStructureExists(afeStructure.Id))
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
            return View(afeStructure);
        }

        // GET: AfeStructures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AfeStructure == null)
            {
                return NotFound();
            }

            var afeStructure = await _context.AfeStructure
                .FirstOrDefaultAsync(m => m.Id == id);
            if (afeStructure == null)
            {
                return NotFound();
            }

            return View(afeStructure);
        }

        // POST: AfeStructures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AfeStructure == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AfeStructure'  is null.");
            }
            var afeStructure = await _context.AfeStructure.FindAsync(id);
            if (afeStructure != null)
            {
                _context.AfeStructure.Remove(afeStructure);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AfeStructureExists(int id)
        {
          return (_context.AfeStructure?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
