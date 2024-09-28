using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class ClientTargetEntityTypeConfiguration : IEntityTypeConfiguration<ClientTarget>
{
    public void Configure(EntityTypeBuilder<ClientTarget> builder)
    {
        builder.HasKey(q => q.Id);

        builder.HasIndex(q => new { q.ClientId, q.Email }).IsUnique();

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.ClientId)
            .IsRequired();

        builder.Property(q => q.Email)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(q => q.FullName)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(q => q.CreatedAt)
            .IsRequired();
        
        builder.HasOne(q => q.Client)
            .WithMany(q => q.Targets)
            .HasForeignKey(q => q.ClientId);
    }
}