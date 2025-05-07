using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Raktar.DataContext.Entities;

namespace Raktar.DataContext;

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

    public virtual DbSet<OrderItem> Carts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<LandRegistryNumber> LandRegistryNumbers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Settlement> Settlements { get; set; }

    public virtual DbSet<SimpleAddress> SimpleAddresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=GERGO\\MSSQLSERVER01;Database=Warehouse;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2AFBE08D2200");
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasKey(e => e.BlockId);

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Blocks__ProductI__403A8C7D");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.OrderId }).HasName("PK__Cart__B40CC6ED54605DBE");

            entity.ToTable("OrderItem");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever();
            entity.Property(e => e.OrderId)
                .ValueGeneratedNever();

            entity.HasOne(d => d.Feedback).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__Cart__FeedbackId__5629CD9C");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart__ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Cart__OrderID__5535A963");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6CB67F50F");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasColumnName("FeedbackID");
            entity.Property(e => e.FeedbackText).HasMaxLength(500);
        });

        modelBuilder.Entity<LandRegistryNumber>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__LandRegi__091C2AFB62847A28");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.Contents)
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.Address).WithOne(p => p.LandRegistryNumber)
                .HasForeignKey<LandRegistryNumber>(d => d.AddressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__LandRegis__Addre__4D94879B");

            entity.HasOne(d => d.Settlement).WithMany(p => p.LandRegistryNumbers)
                .HasForeignKey(d => d.SettlementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LandRegis__Settl__4E88ABD4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFC81DC18F");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.DeliveryAdressId).HasColumnName("DeliveryAdressID");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");

            entity.HasOne(d => d.DeliveryAdress).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryAdressId)
                .HasConstraintName("FK__Orders__Delivery__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserId__5165187F");

            entity.HasMany(d => d.OrderItems).WithOne(p => p.Order)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Orders__OrderItems");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED8D232CD2");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Name).HasMaxLength(350);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Type).HasMaxLength(300);

            entity.HasMany(p => p.OrderItems).WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .HasConstraintName("FK__Products_OrderItems");
            entity.HasMany(p => p.Blocks).WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .HasConstraintName("FK__Products_Blocks");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A3E44BD2B");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);

            entity.HasMany(d => d.Users).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "Privilage",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Privilage__UserI__3C69FB99"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Privilage__RoleI__3B75D760"),
                    j =>
                    {
                        j.HasKey("RoleId", "UserId").HasName("PK__Privilag__5B8242FE629EC484");
                        j.ToTable("Privilages");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.HasKey(e => e.SettlementId).HasName("PK__Settleme__7712545A878A8AE7");

            entity.HasIndex(e => e.PostCode, "idx_PostCode");

            entity.Property(e => e.SettlementName).HasMaxLength(20).IsUnicode(false);
        });

        modelBuilder.Entity<SimpleAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__SimpleAd__091C2AFB241679E9");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.StreetName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.StreetType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Address).WithOne(p => p.SimpleAddress)
                .HasForeignKey<SimpleAddress>(d => d.AddressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SimpleAdd__Addre__49C3F6B7");

            entity.HasOne(d => d.Settlement).WithMany(p => p.SimpleAddresses)
                .HasForeignKey(d => d.SettlementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SimpleAdd__Settl__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C468AD53B");

            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(320);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
