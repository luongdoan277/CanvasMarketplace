using CanvasMarketplace.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CanvasMarketplace.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName).HasMaxLength(50);
            builder.Property(x => x.LastName).HasMaxLength(50);
            builder.Property(x => x.Dob).HasColumnType("date");
         
            builder.HasMany(x => x.Orders).WithOne(x => x.AppUser).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction); ;
            builder.HasMany(x => x.Product).WithOne(x => x.AppUser).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction); ;
        }
    }
}
