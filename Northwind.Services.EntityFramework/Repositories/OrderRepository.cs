using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryCustomerCode = Northwind.Services.Repositories.CustomerCode;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryProduct = Northwind.Services.Repositories.Product;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext context;

    public OrderRepository(NorthwindContext context)
    {
        this.context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        var result = await this.context.Orders
                        .Include(or => or.Shipper)
                        .Include(or => or.Customer)
                        .Include(or => or.Employee)
                        .FirstOrDefaultAsync(or => or.OrderID == orderId);

        if (result == null)
        {
            throw new OrderNotFoundException();
        }

        var rrd = new RepositoryOrder(result.OrderID)
        {
            Customer = new RepositoryCustomer(new RepositoryCustomerCode(result.CustomerID))
            {
                CompanyName = result.Customer.CompanyName,
            },
            Employee = new RepositoryEmployee(result.EmployeeID)
            {
                FirstName = result.Employee.FirstName,
                LastName = result.Employee.LastName,
                Country = result.Employee.Country,
            },
            Shipper = new RepositoryShipper(result.ShipVia)
            {
                CompanyName = result.Shipper.CompanyName,
            },
            ShippingAddress = new ShippingAddress(result.ShipAddress, result.ShipCity, result.ShipRegion, result.ShipPostalCode, result.ShipCountry),
            OrderDate = result.OrderDate,
            RequiredDate = result.RequiredDate,
            ShippedDate = result.ShippedDate,
            Freight = result.Freight,
            ShipName = result.ShipName,
        };

        var details = this.context.OrderDetails
                                .Include(or => or.Product)
                                    .ThenInclude(product => product.Category)
                                .Include(or => or.Product)
                                    .ThenInclude(product => product.Supplier)
                                .Where(or => or.OrderId == orderId);

        foreach (var od in details)
        {
            rrd.OrderDetails.Add(new RepositoryOrderDetail(rrd)
            {
                Product = new RepositoryProduct(od.Product.ProductID)
                {
                    ProductName = od.Product.ProductName,
                    CategoryId = od.Product.Category.CategoryID,
                    Category = od.Product.Category.CategoryName,
                    SupplierId = od.Product.Supplier.SupplierID,
                    Supplier = od.Product.Supplier.CompanyName,
                },
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Discount = od.Discount,
            });
        }

        return rrd;
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), "out of range skip arg");
        }

        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "out of range count arg");
        }

        var list = await this.context.Orders.Skip(skip).Take(count).ToListAsync();
        var result = new List<RepositoryOrder>();
        foreach (var or in list)
        {
            var orderById = await this.GetOrderAsync(or.OrderID);
            result.Add(orderById);
        }

        return result;
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        foreach (var od in order.OrderDetails)
        {
            if (od.Product.Id < 1 || od.UnitPrice < 0 || od.Quantity < 1 || od.Discount < 0)
            {
                throw new RepositoryException("invalid orderDetails input");
            }
        }

        var isExist = await this.context.Orders.FirstOrDefaultAsync(or => or.OrderID == order.Id);
        if (isExist != null)
        {
            return order.Id;
        }

        try
        {
            var newOrder = new Entities.Order()
            {
                OrderID = order.Id,
                CustomerID = order.Customer.Code.Code,
                EmployeeID = order.Employee.Id,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = order.Shipper.Id,
                Freight = order.Freight,
                ShipName = order.Shipper.CompanyName,
                ShipAddress = order.ShippingAddress.Address,
                ShipCity = order.ShippingAddress.City,
                ShipRegion = order.ShippingAddress.Region,
                ShipPostalCode = order.ShippingAddress.PostalCode,
                ShipCountry = order.ShippingAddress.Country,
            };

            foreach (var od in order.OrderDetails)
            {
                var newOrderDetails = new Entities.OrderDetail()
                {
                    OrderId = order.Id,

                    ProductID = od.Product.Id,

                    UnitPrice = od.UnitPrice,

                    Quantity = od.Quantity,

                    Discount = od.Discount,
                };
                _ = this.context.Orders.Add(newOrder);
                _ = this.context.OrderDetails.Add(newOrderDetails);
            }

            _ = await this.context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message);
        }

        return order.Id;
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        var order = this.context.Orders
                        .Include(or => or.OrderDetails)
            .Where(or => or.OrderID == orderId).FirstOrDefault();

        if (order == null)
        {
            throw new OrderNotFoundException();
        }

        _ = this.context.Remove(order);
        _ = await this.context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        var orderById = await this.context.Orders.FirstOrDefaultAsync(or => or.OrderID == order.Id);
        if (orderById is null)
        {
            throw new OrderNotFoundException("order not exist");
        }

        orderById.OrderID = order.Id;
        orderById.CustomerID = order.Customer.Code.Code;
        orderById.EmployeeID = order.Employee.Id;
        orderById.OrderDate = order.OrderDate;
        orderById.RequiredDate = order.RequiredDate;
        orderById.ShippedDate = order.ShippedDate;
        orderById.ShipVia = order.Shipper.Id;
        orderById.Freight = order.Freight;
        orderById.ShipName = order.ShipName;
        orderById.ShipAddress = order.ShippingAddress.Address;
        orderById.ShipCity = order.ShippingAddress.City;
        orderById.ShipRegion = order.ShippingAddress.Region;
        orderById.ShipPostalCode = order.ShippingAddress.PostalCode;
        orderById.ShipCountry = order.ShippingAddress.Country;

        var orderDetailsById = this.context.OrderDetails.Where(or => or.OrderId == order.Id).ToList();
        foreach (var od in orderDetailsById)
        {
            _ = this.context.OrderDetails.Remove(od);
        }

        foreach (var od in order.OrderDetails)
        {
            var newOrderDetails = new Entities.OrderDetail()
            {
                OrderId = order.Id,

                ProductID = od.Product.Id,

                UnitPrice = od.UnitPrice,

                Quantity = od.Quantity,

                Discount = od.Discount,
            };

            _ = this.context.OrderDetails.Add(newOrderDetails);
        }

        _ = await this.context.SaveChangesAsync();
    }
}
