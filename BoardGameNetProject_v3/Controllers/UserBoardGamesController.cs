using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BoardGameNetProject_v3.Data;
using BoardGameNetProject_v3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SQLitePCL;


namespace BoardGameNetProject_v3.Controllers
{
    [Authorize]
    public class UserBoardGamesController : Controller
    {
        private readonly SQLiteDBContext _context;

        public UserBoardGamesController(SQLiteDBContext context)
        {
            _context = context;
        }
        // public async Task<IActionResult> Index()
        // {
        //     var userGames = from ug in _context.UserGames
        //         select ug;
        //     userGames = userGames.Where(ug => ug.UserId.Contains
        //         (User.FindFirstValue(ClaimTypes.NameIdentifier)));
        //     return (_context.BoardGames != null || _context.UserGames != null) ? 
        //         View(await userGames.ToListAsync()) :
        //         Problem("Entity set 'SQLiteDBContext.BoardGames'  is null.");
        // }
        public async Task<IActionResult> Index()
        {
            var userGames = from ug in _context.UserGames
                join bg in _context.BoardGames on ug.boardGameVm.Id equals bg.Id
                select new UserGame
                {
                    Id = ug.Id,
                    UserId = ug.UserId,
                    boardGameVm = ug.boardGameVm,
                    User_rate = ug.User_rate
                };
            // var userGames = from ug in _context.UserGames select ug;
            userGames = userGames.Where(ug => ug.UserId.Contains
                (User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return (_context.BoardGames != null || _context.UserGames != null) ? 
                View(await userGames.ToListAsync()) :
                Problem("Entity set 'SQLiteDBContext.BoardGames'  is null.");
        }
        
        
        public IActionResult Add()
        {
            var bg = _context.BoardGames.ToList();
            if (bg != null)
            {
                ViewBag.data = bg;
            }

            return View();
        }
        //działa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string title)
        {
            if (ModelState.IsValid)
            {

                BoardGame boardGame = _context.BoardGames.FirstOrDefault(db => db.Title == title);
                UserGame userGame = new UserGame();
                userGame.boardGameVm = boardGame;
                userGame.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //check if object exist
                var us_game_list = from ug in _context.UserGames select ug;
                us_game_list = us_game_list.Where(ug => ug.UserId.Contains
                    (User.FindFirstValue(ClaimTypes.NameIdentifier)));
                if (us_game_list.Any(x => x.boardGameVm.Id == boardGame.Id))
                {
                    return RedirectToAction(nameof(Index));
                }
                
                _context.Add(userGame);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
                
            }
            return View("Index");
        }
        //działa
        public async Task<IActionResult> Rate(int? id)
        {
            if (id == null || _context.UserGames == null)
            {
                return NotFound();
            }

            var userGame =  _context.UserGames.FirstOrDefault(x => x.Id == id);
            if (userGame == null)
            {
                return NotFound();
            }
            return View(userGame);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(UserGame userGame)
        {
            var currentEdit = _context.UserGames.FirstOrDefault(p => p.Id == userGame.Id);
            if (currentEdit == null)
                return NotFound();
            if (userGame.User_rate >= 1 && userGame.User_rate <= 10)
            {
                currentEdit.User_rate = userGame.User_rate;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserGames == null)
            {
                return NotFound();
            }

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGame == null)
            {
                return NotFound();
            }

            return View(userGame);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserGame userGame)
        {
            var currentEdit = _context.UserGames.FirstOrDefault(p => p.Id == userGame.Id);
            if (currentEdit == null)
                return NotFound();
            _context.UserGames.Remove(currentEdit);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        

    }
}