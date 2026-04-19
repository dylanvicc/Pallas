using Microsoft.EntityFrameworkCore;
using Pallas.API.Models.Inventory;
using Pallas.API.Models.Items;
using Pallas.API.Models.Locations;
using Pallas.API.Models.Users;

namespace Pallas.API
{
    public class ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<InventoryLevel> InventoryLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDatabaseContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
