using CanvasMarketplace.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CanvasMarketplace.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Product).WithMany(x => x.OrderItems).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction) ;
        }
    }
}
