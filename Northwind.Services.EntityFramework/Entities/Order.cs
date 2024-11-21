using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Orders")]
public class Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long OrderID { get; set; }

    public string CustomerID { get; set; } = string.Empty;

    public long EmployeeID { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public long ShipVia { get; set; }

    public double Freight { get; set; }

    public string ShipName { get; set; } = default!;

    public string ShipAddress { get; set; } = default!;

    public string ShipCity { get; set; } = string.Empty;

    public string? ShipRegion { get; set; }

    public string ShipPostalCode { get; set; } = string.Empty;

    public string ShipCountry { get; set; } = string.Empty;

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    public virtual Customer Customer { get; set; } = default!;

    public virtual Shipper Shipper { get; set; } = default!;

    public virtual Employee Employee { get; set; } = default!;
}
