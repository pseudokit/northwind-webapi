namespace Northwind.Services.EntityFramework.Entities;

public class Category
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public virtual ICollection<Product>? Products { get; set; }
}
