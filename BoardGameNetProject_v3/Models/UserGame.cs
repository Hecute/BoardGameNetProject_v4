using Microsoft.EntityFrameworkCore;

namespace BoardGameNetProject_v3.Models;


public class UserGame
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public BoardGame boardGameVm { get; set; }
    public int User_rate { get; set; }
}