using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class AttackTargetEntityTypeConfiguration : IEntityTypeConfiguration<AttackTarget>
{
    public void Configure(EntityTypeBuilder<AttackTarget> builder)
    {
        builder.HasKey(q => new
        {
            q.AttackId,
            q.TargetEmail
        });

        builder.Property(q => q.AttackId)
            .IsRequired();
        
        builder.Property(q => q.AttackType)
            .HasMaxLength(200);

        builder.Property(q => q.TargetEmail)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(q => q.TargetFullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(q => q.Succeeded)
            .IsRequired();

        builder.HasOne(q => q.Attack)
            .WithMany(q => q.Targets)
            .HasForeignKey(q => q.AttackId);
    }
}