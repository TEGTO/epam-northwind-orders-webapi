using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Shipper
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ShipperID { get; set; }

    [Required]
    public string CompanyName { get; set; } = default!;

    public string? Phone { get; set; }

    public IList<Order> Orders { get; } =
        [];
}
