using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailManagement.API.Models;

public partial class StockTransfer
{
    [Key]
    public int Id { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public DateTime TransferDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("FromWarehouseId")]
    [InverseProperty("StockTransferFromWarehouses")]
    public virtual Warehouse FromWarehouse { get; set; } = null!;

    [InverseProperty("StockTransfer")]
    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    [ForeignKey("ToWarehouseId")]
    [InverseProperty("StockTransferToWarehouses")]
    public virtual Warehouse ToWarehouse { get; set; } = null!;
}
