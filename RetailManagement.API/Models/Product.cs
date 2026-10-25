using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailManagement.API.Models;

[Index("Code", Name = "UQ__Products__A25C5AA7C50E0CEA", IsUnique = true)]
public partial class Product
{
    [Key]
    public int Id { get; set; }


    [StringLength(150)]
    public string Name { get; set; } = null!;


    [StringLength(50)]
    public string Code { get; set; } = null!;


    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CategoryId { get; set; }


    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    [InverseProperty("Product")]
    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();


    [InverseProperty("Product")]
    public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
}
