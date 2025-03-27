using backend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Database;

public class SportClubContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<GymHall> GymHalls { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<GalleryItem> GalleryItems { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public SportClubContext(DbContextOptions<SportClubContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка многие-ко-многим между Schedule и User
        modelBuilder.Entity<Schedule>()
            .HasMany(s => s.Attendees)
            .WithMany(u => u.SchedulesAttended)
            .UsingEntity(j => j.ToTable("ScheduleAttendees"));
        //
        // // Настройка многие-ко-многим между Schedule и User
        // modelBuilder.Entity<Schedule>()
        //     .HasOne(s => s.Coach)
        //     .WithMany(u => u.SchedulesLed);
    }
}