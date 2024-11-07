using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class AttackEmailEntityTypeConfiguration : IEntityTypeConfiguration<AttackEmail>
{
    public void Configure(EntityTypeBuilder<AttackEmail> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property(q => q.ConversationId)
            .IsRequired();

        builder.Property(q => q.State)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(q => q.From)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(q => q.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(q => q.To)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(q => q.Subject)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(q => q.Body)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.TryCount)
            .IsRequired();

        builder.HasOne(q => q.Conversation)
            .WithMany(q => q.AttackEmails)
            .HasForeignKey(q => q.ConversationId);
        
        builder.HasOne(q => q.AttackEmailReply)
            .WithMany()
            .HasForeignKey(q => q.AttackEmailReplyId);
    }
}