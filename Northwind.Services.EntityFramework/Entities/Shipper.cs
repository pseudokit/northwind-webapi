namespace Northwind.Services.EntityFramework.Entities;

public class Shipper
{
    public long ShipperID { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public virtual ICollection<Order>? Orders { get; set; }
}
