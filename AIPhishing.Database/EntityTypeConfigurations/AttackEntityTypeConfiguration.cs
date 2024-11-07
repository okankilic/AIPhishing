using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class AttackEntityTypeConfiguration : IEntityTypeConfiguration<Attack>
{
    public void Configure(EntityTypeBuilder<Attack> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.Language)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(q => q.State)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.HasOne(q => q.Client)
            .WithMany(q => q.Attacks)
            .HasForeignKey(q => q.ClientId);
        
        builder.HasMany(q => q.Conversations)
            .WithOne(q => q.Attack)
            .HasForeignKey(q => q.AttackId);
    }
}