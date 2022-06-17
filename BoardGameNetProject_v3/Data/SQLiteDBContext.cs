using BoardGameNetProject_v3.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameNetProject_v3.Data;

public class SQLiteDBContext:DbContext
{
    public  DbSet<BoardGame?> BoardGames { get; set; }
    public DbSet<UserGame> UserGames { get; set; }

    public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=sqlitedemo.db");
}