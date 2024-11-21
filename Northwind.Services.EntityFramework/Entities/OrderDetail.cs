using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("OrderDetails")]
public class OrderDetail
{
    public long OrderId { get; set; }

    public long ProductID { get; set; }

    public double UnitPrice { get; set; }

    public long Quantity { get; set; }

    public double Discount { get; set; }

    public virtual Order Order { get; set; } = default!;

    public virtual Product Product { get; set; } = default!;
}
