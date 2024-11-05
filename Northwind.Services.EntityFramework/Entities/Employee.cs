using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long EmployeeID { get; set; }

    [Required]
    public string LastName { get; set; } = default!;

    [Required]
    public string FirstName { get; set; } = default!;

    public string? Title { get; set; }

    public string? TitleOfCourtesy { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? HireDate { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    [Required]
    public string Country { get; set; } = default!;

    public string? HomePhone { get; set; }

    public string? Extension { get; set; }

    public string? Notes { get; set; }

    public long? ReportsTo { get; set; }

    [ForeignKey("EmployeeID")]
    public Employee? Manager { get; set; }

    public IList<Employee> Subordinates { get; } =
        [];

    public IList<Order> Orders { get; } =
        [];
}
