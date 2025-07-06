using ChattingAppAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.SourceUser)
            .WithMany(u => u.LikedUsers)
            .HasForeignKey(ul => ul.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.TargetUser)
            .WithMany(u => u.LikedByUsers)
            .HasForeignKey(ul => ul.TargetUserId)
            //noAction for avoiding casacde cycles
            //target user must be deleted manually
            .OnDelete(DeleteBehavior.NoAction);
    }
}





