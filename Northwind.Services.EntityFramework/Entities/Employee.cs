using System.ComponentModel.DataAnnotations;

namespace Northwind.Services.EntityFramework.Entities;

public class Employee
{
    public long EmployeeID { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string TitleOfCourtesy { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }

    public DateTime? HireDate { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? HomePhone { get; set; }

    public string? Extension { get; set; }

    public string? Notes { get; set; }

    public int? ReportsTo { get; set; }

    public string? PhotoPath { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = default!;
}
