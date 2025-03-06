using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Context;

public partial class WarehouseDbContext : DbContext
{
    public WarehouseDbContext()
    {
    }

    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<LandRegistryNumber> LandRegistryNumbers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Privilage> Privilages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Settlement> Settlements { get; set; }

    public virtual DbSet<SimpleAddress> SimpleAddresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=WarehouseDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2AFB84C7EA3B");
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Blocks__ProductI__403A8C7D");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Cart__B40CC6EDD8C1DB24");

            entity.ToTable("Cart");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Feedback).WithMany(p => p.Carts)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__Cart__FeedbackId__5629CD9C");

            entity.HasOne(d => d.Order).WithMany(p => p.Carts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Cart__OrderID__5535A963");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6DF8EF7F4");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .ValueGeneratedNever()
                .HasColumnName("FeedbackID");
            entity.Property(e => e.FeedbackText).HasMaxLength(500);
        });

        modelBuilder.Entity<LandRegistryNumber>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__LandRegi__091C2AFB9AAC710C");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.Contents)
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.Address).WithOne(p => p.LandRegistryNumber)
                .HasForeignKey<LandRegistryNumber>(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LandRegis__Addre__4D94879B");

            entity.HasOne(d => d.Settlement).WithMany(p => p.LandRegistryNumbers)
                .HasForeignKey(d => d.SettlementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LandRegis__Settl__4E88ABD4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFE90EE8DC");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.DeliveryAdressId).HasColumnName("DeliveryAdressID");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false);

            entity.HasOne(d => d.DeliveryAdress).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryAdressId)
                .HasConstraintName("FK__Orders__Delivery__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserId__5165187F");
        });

        modelBuilder.Entity<Privilage>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Privilag__8AFACE3A2ECAB26B");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithOne(p => p.Privilage)
                .HasForeignKey<Privilage>(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Privilage__RoleI__3B75D760");

            entity.HasOne(d => d.User).WithMany(p => p.Privilages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Privilage__UserI__3C69FB99");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6EDD81B407D");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.Name).HasMaxLength(350);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Type).HasMaxLength(300);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A4DA9AC5C");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.HasKey(e => e.SettlementId).HasName("PK__Settleme__7712545AF4762AE8");

            entity.HasIndex(e => e.PostCode, "idx_PostCode");

            entity.Property(e => e.SettlementId).ValueGeneratedNever();
            entity.Property(e => e.SettlementName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SimpleAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__SimpleAd__091C2AFB9D3C7D8D");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.StreetName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StreetType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Address).WithOne(p => p.SimpleAddress)
                .HasForeignKey<SimpleAddress>(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SimpleAdd__Addre__49C3F6B7");

            entity.HasOne(d => d.Settlement).WithMany(p => p.SimpleAddresses)
                .HasForeignKey(d => d.SettlementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SimpleAdd__Settl__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5E98F91D");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(320);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
