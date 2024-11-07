using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPhishing.Database.EntityTypeConfigurations;

public class ConversationEntityTypeConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new
        {
            e.ClientTargetId,
            e.AttackId
        }).IsUnique();
        
        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property(q => q.ClientTargetId)
            .IsRequired();
        
        builder.Property(q => q.AttackId)
            .IsRequired();
        
        builder.Property(q => q.AttackType)
            .HasMaxLength(200);
        
        builder.Property(q => q.Sender)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(q => q.Subject)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(e => e.ClientTarget)
            .WithMany(ct => ct.Conversations)
            .HasForeignKey(e => e.ClientTargetId);
        
        builder.HasOne(e => e.Attack)
            .WithMany(a => a.Conversations)
            .HasForeignKey(e => e.AttackId);

        builder.HasMany(e => e.AttackEmails)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);
        
        builder.HasMany(e => e.AttackEmailReplies)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);
    }
}