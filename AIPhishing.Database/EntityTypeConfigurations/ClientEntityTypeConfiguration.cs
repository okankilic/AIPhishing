using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(q => q.Id);

        builder.HasIndex(q => q.ClientName).IsUnique();

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.ClientName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.HasMany(q => q.Users)
            .WithOne(q => q.Client)
            .HasForeignKey(q => q.ClientId);
        
        builder.HasMany(q => q.Targets)
            .WithOne(q => q.Client)
            .HasForeignKey(q => q.ClientId);
        
        builder.HasMany(q => q.Attacks)
            .WithOne(q => q.Client)
            .HasForeignKey(q => q.ClientId);
    }
}