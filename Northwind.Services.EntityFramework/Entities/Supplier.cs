using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Suppliers")]
public class Supplier
{
    public int SupplierID { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? ContactName { get; set; } = string.Empty;

    public string? ContactTitle { get; set; } = string.Empty;

    public string? Address { get; set; } = string.Empty;

    public string? City { get; set; } = string.Empty;

    public string? Region { get; set; } = string.Empty;

    public string? PostalCode { get; set; } = string.Empty;

    public string? Country { get; set; } = string.Empty;

    public string? Phone { get; set; } = string.Empty;

    public string? Fax { get; set; } = string.Empty;

    public string? HomePage { get; set; }

    public virtual ICollection<Product> Products { get; set; } = default!;
}
