using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailManagement.API.Models;

[Index("WarehouseId", "ProductId", Name = "UQ_WarehouseStocks_Warehouse_Product", IsUnique = true)]
public partial class WarehouseStock
{
    [Key]
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("WarehouseStocks")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("WarehouseStocks")]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
