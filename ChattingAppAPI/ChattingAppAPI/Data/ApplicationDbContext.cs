using ChattingAppAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }

}





