using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Shipper> Shippers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<OrderDetail>()
                .HasKey(rd => new { rd.OrderId, rd.ProductID });

        _ = modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product);

        _ = modelBuilder.Entity<Product>()
                        .HasOne(p => p.Supplier)
                        .WithMany(s => s.Products);

        _ = modelBuilder.Entity<Product>()
                        .HasOne(p => p.Category)
                        .WithMany(s => s.Products);

        _ = modelBuilder.Entity<Product>()
                        .HasKey(p => p.ProductID);

        _ = modelBuilder.Entity<Order>()
                        .HasOne(or => or.Customer)
                        .WithMany(c => c.Orders)
                        .HasForeignKey(or => or.CustomerID);

        _ = modelBuilder.Entity<Order>()
                        .HasOne(or => or.Shipper)
                        .WithMany(c => c.Orders)
                        .HasForeignKey(or => or.ShipVia);

        _ = modelBuilder.Entity<Order>()
                        .HasOne(or => or.Employee)
                        .WithMany(c => c.Orders)
                        .HasForeignKey(or => or.EmployeeID);

        _ = modelBuilder.Entity<Order>()
                        .HasMany(or => or.OrderDetails)
                        .WithOne(od => od.Order);
    }
}
