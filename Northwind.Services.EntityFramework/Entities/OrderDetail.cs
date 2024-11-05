using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

[PrimaryKey(nameof(OrderID), nameof(ProductID))]
public class OrderDetail
{
    public long OrderID { get; set; }

    public Order Order { get; set; } = default!;

    public long ProductID { get; set; }

    public Product Product { get; set; } = default!;

    [Required]
    public double UnitPrice { get; set; }

    [Required]
    public long Quantity { get; set; }

    [Required]
    public double Discount { get; set; }
}
