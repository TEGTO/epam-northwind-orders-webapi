using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long OrderID { get; set; }

    [Required]
    public string CustomerID { get; set; } = default!;

    public Customer Customer { get; set; } = default!;

    [Required]
    public long EmployeeID { get; set; }

    public Employee Employee { get; set; } = default!;

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public DateTime RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    [Required]
    public long ShipVia { get; set; }

    [ForeignKey("ShipVia")]
    public Shipper Shipper { get; set; } = default!;

    [Required]
    public double Freight { get; set; }

    [Required]
    public string ShipName { get; set; } = default!;

    [Required]
    public string ShipAddress { get; set; } = default!;

    [Required]
    public string ShipCity { get; set; } = default!;

    public string? ShipRegion { get; set; }

    [Required]
    public string ShipPostalCode { get; set; } = default!;

    [Required]
    public string ShipCountry { get; set; } = default!;

    public IList<OrderDetail> OrderDetails { get; } =
        [];
}
