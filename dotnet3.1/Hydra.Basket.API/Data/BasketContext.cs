using System.Linq;
using Hydra.Basket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Hydra.Basket.API.Data
{
    public class BasketContext : DbContext
    {
        public BasketContext(DbContextOptions<BasketContext> options) : base(options){ 
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<BasketItem> BasketItems {get; set; }
        public DbSet<BasketCustomer> BasketCustomers {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.Entity<BasketCustomer>()
                        .HasIndex(c => c.CustomerId)
                        .HasName("IDX_Customer");
            
            modelBuilder.Entity<BasketCustomer>()
                        .HasMany(c => c.Items)
                        .WithOne(c => c.BasketCustomer)
                        .HasForeignKey(c => c.BasketId);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
        }
    }
}