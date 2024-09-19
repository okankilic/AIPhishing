using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class AttackEmailReplyEntityTypeConfiguration : IEntityTypeConfiguration<AttackEmailReply>
{
    public void Configure(EntityTypeBuilder<AttackEmailReply> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.AttackEmailId)
            .IsRequired();
        
        builder.Property(q => q.Subject)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(q => q.Body)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.HasOne(q => q.AttackEmail)
            .WithMany(q => q.Replies)
            .HasForeignKey(q => q.AttackEmailId);
    }
}