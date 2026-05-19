﻿using Microsoft.EntityFrameworkCore;

namespace EventBookingSystem.Models
{
    
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) { }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<User > Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure a unique constraint on the User's email address.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();

            // Configure the one-to-many relationship between User and Booking.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.user_id)
                .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a user if they have bookings.

            // Configure the one-to-many relationship between Event and Booking.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.event_id)
                .OnDelete(DeleteBehavior.Restrict); // Prevents deleting an event if it has bookings.
        }
    }
}
