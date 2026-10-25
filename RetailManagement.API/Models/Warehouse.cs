using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailManagement.API.Models;

public partial class Warehouse
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("FromWarehouse")]
    public virtual ICollection<StockTransfer> StockTransferFromWarehouses { get; set; } = new List<StockTransfer>();

    [InverseProperty("ToWarehouse")]
    public virtual ICollection<StockTransfer> StockTransferToWarehouses { get; set; } = new List<StockTransfer>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
}
