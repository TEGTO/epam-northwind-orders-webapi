using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProductID { get; set; }

    [Required]
    public string ProductName { get; set; } = default!;

    [Required]
    public long SupplierID { get; set; }

    public Supplier Supplier { get; set; } = default!;

    [Required]
    public long CategoryID { get; set; }

    public Category Category { get; set; } = default!;

    public string? QuantityPerUnit { get; set; }

    [Required]
    public double UnitPrice { get; set; }

    [Required]
    public int UnitsInStock { get; set; }

    [Required]
    public int UnitsOnOrder { get; set; }

    [Required]
    public int ReorderLevel { get; set; }

    [Required]
    public bool Discontinued { get; set; }

    public IList<OrderDetail> OrderDetails { get; } =
        [];
}
