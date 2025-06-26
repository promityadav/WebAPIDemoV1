using Microsoft.EntityFrameworkCore;
using WebAPIDemo.Authority;
using WebAPIDemo.Models;

namespace WebAPIDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<Shirt> Shirt { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // data seeding

            modelBuilder.Entity<Shirt>().HasData(
                new Shirt { shirtId = 1, Brand = "My Brand 1", color = "Blue", Gender = "Men", Price = 20, Size = 10 },
                new Shirt { shirtId = 2, Brand = "My Brand 2", color = "Black", Gender = "Men", Price = 30, Size = 12 },
                new Shirt { shirtId = 3, Brand = "My Brand 3", color = "Pink", Gender = "Women", Price = 10, Size = 8 },
                new Shirt { shirtId = 4, Brand = "My Brand 4", color = "Yellow", Gender = "Wome", Price = 25, Size = 9 }
                );
        }

    }
}
