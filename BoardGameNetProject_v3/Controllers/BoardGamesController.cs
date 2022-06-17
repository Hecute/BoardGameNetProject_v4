using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoardGameNetProject_v3.Data;
using BoardGameNetProject_v3.Models;
using Microsoft.AspNetCore.Authorization;

namespace BoardGameNetProject_v3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BoardGamesController : Controller
    {
        private readonly SQLiteDBContext _context;

        public BoardGamesController(SQLiteDBContext context)
        {
            _context = context;
        }

        // GET: BoardGames
        public async Task<IActionResult> Index()
        {
              return _context.BoardGames != null ? 
                          View(await _context.BoardGames.ToListAsync()) :
                          Problem("Entity set 'SQLiteDBContext.BoardGames'  is null.");
        }

        // GET: BoardGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BoardGames == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // GET: BoardGames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardGames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Rating")] BoardGame boardGame)
        {
            if (ModelState.IsValid)
            {
                if (!_context.BoardGames.Any(x => x.Title == boardGame.Title))
                {
                    _context.Add(boardGame);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(boardGame);
        }

        // GET: BoardGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BoardGames == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame == null)
            {
                return NotFound();
            }
            return View(boardGame);
        }

        // POST: BoardGames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Rating")] BoardGame boardGame)
        {
            if (id != boardGame.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardGameExists(boardGame.Id))
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
            return View(boardGame);
        }

        // GET: BoardGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BoardGames == null)
            {
                return NotFound();
            }

            var boardGame = await _context.BoardGames
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        // POST: BoardGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BoardGames == null)
            {
                return Problem("Entity set 'SQLiteDBContext.BoardGames'  is null.");
            }
            var boardGame = await _context.BoardGames.FindAsync(id);
            if (boardGame != null)
            {
                _context.BoardGames.Remove(boardGame);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardGameExists(int id)
        {
          return (_context.BoardGames?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
