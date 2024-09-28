using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(q => q.Id);

        builder.HasIndex(q => q.Email).IsUnique();

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.Email)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(q => q.Password)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(q => q.CreatedAt)
            .IsRequired();
        
        builder.HasOne(q => q.Client)
            .WithMany(q => q.Users)
            .HasForeignKey(q => q.ClientId);
    }
}