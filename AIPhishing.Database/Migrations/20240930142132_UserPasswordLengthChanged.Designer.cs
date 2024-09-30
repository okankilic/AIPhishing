﻿// <auto-generated />
using System;
using AIPhishing.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    [DbContext(typeof(PhishingDbContext))]
    [Migration("20240930142132_UserPasswordLengthChanged")]
    partial class UserPasswordLengthChanged
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("ai_phishing")
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AIPhishing.Database.Entities.Attack", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("character varying(2)")
                        .HasColumnName("language");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_time");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<string>("Template")
                        .HasColumnType("text")
                        .HasColumnName("template");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_attacks");

                    b.ToTable("attacks", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackEmail", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AttackId")
                        .HasColumnType("uuid")
                        .HasColumnName("attack_id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<DateTime?>("ClickedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("clicked_at");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("display_name");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("from");

                    b.Property<bool>("IsClicked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_clicked");

                    b.Property<bool>("IsOpened")
                        .HasColumnType("boolean")
                        .HasColumnName("is_opened");

                    b.Property<DateTime?>("OpenedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("opened_at");

                    b.Property<DateTime?>("SendAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("send_at");

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sent_at");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("subject");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("to");

                    b.Property<int>("TryCount")
                        .HasColumnType("integer")
                        .HasColumnName("try_count");

                    b.HasKey("Id")
                        .HasName("pk_attack_emails");

                    b.HasIndex("AttackId")
                        .HasDatabaseName("ix_attack_emails_attack_id");

                    b.ToTable("attack_emails", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackEmailReply", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AttackEmailId")
                        .HasColumnType("uuid")
                        .HasColumnName("attack_email_id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("subject");

                    b.HasKey("Id")
                        .HasName("pk_attack_email_replies");

                    b.HasIndex("AttackEmailId")
                        .HasDatabaseName("ix_attack_email_replies_attack_email_id");

                    b.ToTable("attack_email_replies", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackTarget", b =>
                {
                    b.Property<Guid>("AttackId")
                        .HasColumnType("uuid")
                        .HasColumnName("attack_id");

                    b.Property<string>("TargetEmail")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("target_email");

                    b.Property<string>("AttackType")
                        .HasColumnType("text")
                        .HasColumnName("attack_type");

                    b.Property<bool>("Succeeded")
                        .HasColumnType("boolean")
                        .HasColumnName("succeeded");

                    b.Property<string>("TargetFullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("target_full_name");

                    b.HasKey("AttackId", "TargetEmail")
                        .HasName("pk_attack_targets");

                    b.ToTable("attack_targets", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("client_name");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.HasKey("Id")
                        .HasName("pk_clients");

                    b.HasIndex("ClientName")
                        .IsUnique()
                        .HasDatabaseName("ix_clients_client_name");

                    b.ToTable("clients", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.ClientTarget", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("full_name");

                    b.HasKey("Id")
                        .HasName("pk_client_targets");

                    b.HasIndex("ClientId", "Email")
                        .IsUnique()
                        .HasDatabaseName("ix_client_targets_client_id_email");

                    b.ToTable("client_targets", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("ClientId")
                        .HasDatabaseName("ix_users_client_id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.ToTable("users", "ai_phishing");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackEmail", b =>
                {
                    b.HasOne("AIPhishing.Database.Entities.Attack", "Attack")
                        .WithMany("Emails")
                        .HasForeignKey("AttackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_attack_emails_attacks_attack_id");

                    b.Navigation("Attack");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackEmailReply", b =>
                {
                    b.HasOne("AIPhishing.Database.Entities.AttackEmail", "AttackEmail")
                        .WithMany("Replies")
                        .HasForeignKey("AttackEmailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_attack_email_replies_attack_emails_attack_email_id");

                    b.Navigation("AttackEmail");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackTarget", b =>
                {
                    b.HasOne("AIPhishing.Database.Entities.Attack", "Attack")
                        .WithMany("Targets")
                        .HasForeignKey("AttackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_attack_targets_attacks_attack_id");

                    b.Navigation("Attack");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.ClientTarget", b =>
                {
                    b.HasOne("AIPhishing.Database.Entities.Client", "Client")
                        .WithMany("Targets")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_client_targets_clients_client_id");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.User", b =>
                {
                    b.HasOne("AIPhishing.Database.Entities.Client", "Client")
                        .WithMany("Users")
                        .HasForeignKey("ClientId")
                        .HasConstraintName("fk_users_clients_client_id");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.Attack", b =>
                {
                    b.Navigation("Emails");

                    b.Navigation("Targets");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.AttackEmail", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("AIPhishing.Database.Entities.Client", b =>
                {
                    b.Navigation("Targets");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}