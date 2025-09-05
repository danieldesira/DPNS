using System;
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

    public virtual DbSet<PushNotification> PushNotifications { get; set; }

    public virtual DbSet<PushSubscription> PushSubscriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PushNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("push_notifications_pk");

            entity.ToTable("push_notifications");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
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
            entity.Property(e => e.Auth)
                .HasColumnType("character varying")
                .HasColumnName("auth");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Endpoint)
                .HasColumnType("character varying")
                .HasColumnName("endpoint");
            entity.Property(e => e.P256dh)
                .HasColumnType("character varying")
                .HasColumnName("p256dh");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
