using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RetailManagement.API.Models;

namespace RetailManagement.API.Data;

public partial class RetailDbContext : DbContext
{
    public RetailDbContext(DbContextOptions<RetailDbContext> options) : base(options)
    {
    }


    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<StockTransfer> StockTransfers { get; set; }

    public virtual DbSet<StockTransferItem> StockTransferItems { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseStock> WarehouseStocks { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC075B2C0185");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC072E7F40D7");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07E702120E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockTra__3214EC0742668720");

            entity.Property(e => e.Status).HasDefaultValue("Completed");
            entity.Property(e => e.TransferDate).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockTransferFromWarehouses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransfers_FromWarehouse");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockTransferToWarehouses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransfers_ToWarehouse");
        });

        modelBuilder.Entity<StockTransferItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockTra__3214EC0714E7C6BD");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransferItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransferItems_Products");

            entity.HasOne(d => d.StockTransfer).WithMany(p => p.StockTransferItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTransferItems_StockTransfers");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC07012A224A");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<WarehouseStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC075922126A");

            entity.HasOne(d => d.Product).WithMany(p => p.WarehouseStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseStocks_Products");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseStocks_Warehouses");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
