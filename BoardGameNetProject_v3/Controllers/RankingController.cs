using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BoardGameNetProject_v3.Data;
using BoardGameNetProject_v3.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameNetProject_v3.Controllers
{
    public class RankingController : Controller
    {
        private readonly SQLiteDBContext _context;

        public RankingController(SQLiteDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (_context.BoardGames != null)
            {
                var joinedList = from bg in _context.BoardGames
                    join ug in _context.UserGames on bg.Id equals ug.boardGameVm.Id
                    select new
                    {
                        userId = ug.UserId,
                        boardGameId = bg.Id,
                        user_Rating = ug.User_rate
                    };

                var user_counter = 0;
                var rate_sum = 0;
                foreach (var v in _context.BoardGames)
                {
                    var bgrateList = joinedList.Where(x => x.boardGameId == v.Id);
                    foreach (var x in bgrateList)
                    {
                        if (x.user_Rating != 0)
                        {
                            user_counter++;
                            rate_sum += x.user_Rating;
                        }
                    }

                    if (user_counter != 0) v.Rating = rate_sum / user_counter;
                    else v.Rating = 0;
                    user_counter = rate_sum = 0;

                }

                _context.SaveChanges();
            }

            return _context.BoardGames != null ? 
                View(await _context.BoardGames.ToListAsync()) :
                Problem("Entity set 'SQLiteDBContext.BoardGames'  is null.");
        }
        // public ActionResult Index()
        // {
        //     var boardGames = from bg in _context.BoardGames select bg;
        //     boardGames = boardGames.OrderByDescending(bg => bg.Rating);
        //     return View(boardGames.ToList());
        // }
    }
}