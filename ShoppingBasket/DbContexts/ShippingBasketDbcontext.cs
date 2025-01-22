using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Models;

namespace ShoppingBasket.DbContexts
{
    // DbContext
    public class ShoppingBasketDbContext : DbContext
    {
        public DbSet<ShoppingBaskett> ShoppingBaskets { get; set; }
        public DbSet<BasketLine> BasketLines { get; set; }

        public ShoppingBasketDbContext(DbContextOptions<ShoppingBasketDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasketLine>()
                .HasOne<ShoppingBaskett>()
                .WithMany()
                .HasForeignKey(b => b.ShoppingBasketId);

            
        }
    }
}




