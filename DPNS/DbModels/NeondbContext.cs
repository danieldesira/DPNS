﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DPNS.DbModels;

public partial class NeondbContext : DbContext
{
    public NeondbContext()
    {
    }

    public NeondbContext(DbContextOptions<NeondbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<App> Apps { get; set; }

    public virtual DbSet<PushNotification> PushNotifications { get; set; }

    public virtual DbSet<PushSubscription> PushSubscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_pk");

            entity.ToTable("apps");

            entity.HasIndex(e => e.AppName, "projects_unique").IsUnique();

            entity.HasIndex(e => e.Guid, "projects_unique_uuid").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppName)
                .HasColumnType("character varying")
                .HasColumnName("app_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("time with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Guid).HasColumnName("guid");
            entity.Property(e => e.Url)
                .HasColumnType("character varying")
                .HasColumnName("url");
        });

        modelBuilder.Entity<PushNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("push_notifications_pk");

            entity.ToTable("push_notifications");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AppUrl)
                .HasColumnType("character varying")
                .HasColumnName("app_url");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Text)
                .HasColumnType("character varying")
                .HasColumnName("text");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<PushSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("push_subscriptions_pk");

            entity.ToTable("push_subscriptions");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.Auth)
                .HasColumnType("character varying")
                .HasColumnName("auth");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Endpoint)
                .HasColumnType("character varying")
                .HasColumnName("endpoint");
            entity.Property(e => e.P256dh)
                .HasColumnType("character varying")
                .HasColumnName("p256dh");

            entity.HasOne(d => d.App).WithMany(p => p.PushSubscriptions)
                .HasForeignKey(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("push_subscriptions_apps_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("time with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("time with time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
