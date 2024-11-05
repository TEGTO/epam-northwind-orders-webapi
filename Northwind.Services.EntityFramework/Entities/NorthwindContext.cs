using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

public class NorthwindContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Shipper> Shippers { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ExceptionHelper.ThrowArgumentNullException(modelBuilder);

        _ = modelBuilder.Entity<Order>()
                    .HasMany(o => o.OrderDetails)
                    .WithOne(d => d.Order)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Order>()
                    .HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerID)
                    .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Order>()
                    .HasOne(o => o.Shipper)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.ShipVia)
                    .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Order>()
                    .HasOne(o => o.Employee)
                    .WithMany(e => e.Orders)
                    .HasForeignKey(o => o.EmployeeID)
                    .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Product>()
                    .HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierID)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Product>()
                    .HasOne(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryID)
                    .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Employee>()
                    .HasOne(e => e.Manager)
                    .WithMany(e => e.Subordinates)
                    .HasForeignKey(e => e.ReportsTo)
                    .OnDelete(DeleteBehavior.SetNull);

        _ = modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductID)
                    .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
}
