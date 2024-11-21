namespace Northwind.Services.EntityFramework.Entities;

public class Product
{
    public long ProductID { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string QuantityPerUnit { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public int UnitsOnOrder { get; set; }

    public int ReorderLevel { get; set; }

    public int Discontinued { get; set; }

    public virtual Supplier Supplier { get; set; } = default!;

    public virtual Category Category { get; set; } = default!;

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
}
