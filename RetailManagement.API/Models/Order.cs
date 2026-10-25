using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RetailManagement.API.Models;

public partial class Order
{
    [Key]
    public int Id { get; set; }


    [StringLength(150)]
    public string CustomerName { get; set; } = null!;
    public DateTime OrderDate { get; set; }


    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }


    [StringLength(20)]
    public string Status { get; set; } = null!;


    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
