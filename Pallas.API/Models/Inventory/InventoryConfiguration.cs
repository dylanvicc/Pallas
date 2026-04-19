using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pallas.API.Models.Inventory
{
    public class InventoryLevelConfiguration : IEntityTypeConfiguration<InventoryLevel>
    {
        public void Configure(EntityTypeBuilder<InventoryLevel> entity)
        {
            entity.ToTable("inventory_levels", table => table.HasCheckConstraint("inventory_levels_quantity_check", "quantity >= 0"));
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
            entity.Property(e => e.LocationId).HasColumnName("location_id").IsRequired();
            entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(e => e.ReorderPoint).HasColumnName("reorder_point");
            entity.Property(e => e.ReorderQty).HasColumnName("reorder_qty");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.HasOne(e => e.Item).WithMany().HasForeignKey(e => e.ItemId);
            entity.HasOne(e => e.Location).WithMany().HasForeignKey(e => e.LocationId);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy);
            entity.HasOne(e => e.UpdatedByUser).WithMany().HasForeignKey(e => e.UpdatedBy);
            entity.HasIndex(e => new { e.ItemId, e.LocationId }).IsUnique();
        }
    }
}