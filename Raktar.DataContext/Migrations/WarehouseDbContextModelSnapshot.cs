﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Raktar.DataContext;

#nullable disable

namespace Raktar.DataContext.Migrations
{
    [DbContext(typeof(WarehouseDbContext))]
    partial class WarehouseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Privilage", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("RoleId", "UserId")
                        .HasName("PK__Privilag__5B8242FE629EC484");

                    b.HasIndex("UserId");

                    b.ToTable("Privilages", (string)null);
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"));

                    b.HasKey("AddressId")
                        .HasName("PK__Addresse__091C2AFBE08D2200");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Block", b =>
                {
                    b.Property<int>("StorageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StorageId"));

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("StorageId");

                    b.HasIndex("ProductId");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("FeedbackID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<string>("FeedbackText")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("FeedbackId")
                        .HasName("PK__Feedback__6A4BEDF6CB67F50F");

                    b.ToTable("Feedback", (string)null);
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.LandRegistryNumber", b =>
                {
                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasMaxLength(60)
                        .IsUnicode(false)
                        .HasColumnType("varchar(60)");

                    b.Property<int>("SettlementId")
                        .HasColumnType("int");

                    b.HasKey("AddressId")
                        .HasName("PK__LandRegi__091C2AFB62847A28");

                    b.HasIndex("SettlementId");

                    b.ToTable("LandRegistryNumbers");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OrderID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int>("DeliveryAdressId")
                        .HasColumnType("int")
                        .HasColumnName("DeliveryAdressID");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrderId")
                        .HasName("PK__Orders__C3905BAFC81DC18F");

                    b.HasIndex("DeliveryAdressId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.OrderItem", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("FeedbackId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "OrderId")
                        .HasName("PK__Cart__B40CC6ED54605DBE");

                    b.HasIndex("FeedbackId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItem", (string)null);
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int?>("MaxQuantityPerBlock")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(350)
                        .HasColumnType("nvarchar(350)");

                    b.Property<decimal>("Price")
                        .HasColumnType("money");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("ProductId")
                        .HasName("PK__Product__B40CC6ED8D232CD2");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RoleId")
                        .HasName("PK__Role__8AFACE3A3E44BD2B");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Settlement", b =>
                {
                    b.Property<int>("SettlementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SettlementId"));

                    b.Property<int>("PostCode")
                        .HasColumnType("int");

                    b.Property<string>("SettlementName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("SettlementId")
                        .HasName("PK__Settleme__7712545A878A8AE7");

                    b.HasIndex(new[] { "PostCode" }, "idx_PostCode");

                    b.ToTable("Settlements");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.SimpleAddress", b =>
                {
                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("DoorNumber")
                        .HasColumnType("int");

                    b.Property<int?>("FloorNumber")
                        .HasColumnType("int");

                    b.Property<int>("HouseNumber")
                        .HasColumnType("int");

                    b.Property<int>("SettlementId")
                        .HasColumnType("int");

                    b.Property<int?>("StairwayNumber")
                        .HasColumnType("int");

                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("StreetType")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("int");

                    b.HasKey("AddressId")
                        .HasName("PK__SimpleAd__091C2AFB241679E9");

                    b.HasIndex("SettlementId");

                    b.ToTable("SimpleAddresses");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varbinary(128)");

                    b.Property<int>("TelephoneNumber")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CC4C468AD53B");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Privilage", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK__Privilage__RoleI__3B75D760");

                    b.HasOne("Raktar.DataContext.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Privilage__UserI__3C69FB99");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Block", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Product", "Product")
                        .WithMany("Blocks")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__Products_Blocks");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.LandRegistryNumber", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Address", "Address")
                        .WithOne("LandRegistryNumber")
                        .HasForeignKey("Raktar.DataContext.Entities.LandRegistryNumber", "AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__LandRegis__Addre__4D94879B");

                    b.HasOne("Raktar.DataContext.Entities.Settlement", "Settlement")
                        .WithMany("LandRegistryNumbers")
                        .HasForeignKey("SettlementId")
                        .IsRequired()
                        .HasConstraintName("FK__LandRegis__Settl__4E88ABD4");

                    b.Navigation("Address");

                    b.Navigation("Settlement");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Order", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Address", "DeliveryAdress")
                        .WithMany("Orders")
                        .HasForeignKey("DeliveryAdressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Orders__Delivery__52593CB8");

                    b.HasOne("Raktar.DataContext.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Orders__UserId__5165187F");

                    b.Navigation("DeliveryAdress");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.OrderItem", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Feedback", "Feedback")
                        .WithMany("OrderItems")
                        .HasForeignKey("FeedbackId")
                        .HasConstraintName("FK__Cart__FeedbackId__5629CD9C");

                    b.HasOne("Raktar.DataContext.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Orders__OrderItems");

                    b.HasOne("Raktar.DataContext.Entities.Product", "Product")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Products_OrderItems");

                    b.Navigation("Feedback");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.SimpleAddress", b =>
                {
                    b.HasOne("Raktar.DataContext.Entities.Address", "Address")
                        .WithOne("SimpleAddress")
                        .HasForeignKey("Raktar.DataContext.Entities.SimpleAddress", "AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__SimpleAdd__Addre__49C3F6B7");

                    b.HasOne("Raktar.DataContext.Entities.Settlement", "Settlement")
                        .WithMany("SimpleAddresses")
                        .HasForeignKey("SettlementId")
                        .IsRequired()
                        .HasConstraintName("FK__SimpleAdd__Settl__4AB81AF0");

                    b.Navigation("Address");

                    b.Navigation("Settlement");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Address", b =>
                {
                    b.Navigation("LandRegistryNumber");

                    b.Navigation("Orders");

                    b.Navigation("SimpleAddress");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Feedback", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Product", b =>
                {
                    b.Navigation("Blocks");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.Settlement", b =>
                {
                    b.Navigation("LandRegistryNumbers");

                    b.Navigation("SimpleAddresses");
                });

            modelBuilder.Entity("Raktar.DataContext.Entities.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
