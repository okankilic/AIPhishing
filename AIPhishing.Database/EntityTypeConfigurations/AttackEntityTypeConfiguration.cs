﻿using AIPhishing.Database.Entities;
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

        builder.HasMany(q => q.Targets)
            .WithOne(q => q.Attack)
            .HasForeignKey(q => q.AttackId);
        
        builder.HasMany(q => q.Emails)
            .WithOne(q => q.Attack)
            .HasForeignKey(q => q.AttackId);
    }
}